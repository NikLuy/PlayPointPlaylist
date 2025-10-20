using Microsoft.EntityFrameworkCore;
using PlayPointPlaylist.Data;
using PlayPointPlaylist.Models;

namespace PlayPointPlaylist.Services;

public class QueueService
{
    private readonly IDbContextFactory<PlaylistDbContext> _contextFactory;
    
    public QueueService(IDbContextFactory<PlaylistDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<QueueItem> AddSongToPlaylist(string eventUid, string videoId, string title, string artist, int durationSeconds, string requestedBy = "")
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = await context.Events
            .FirstOrDefaultAsync(e => e.UniqueId == eventUid && e.IsActive);
        
        if (evt == null)
            throw new InvalidOperationException("Event not found or inactive");
        
        // Get the next PlayOrder
        var maxOrder = await context.QueueItems
            .Where(q => q.EventId == evt.Id)
            .MaxAsync(q => (int?)q.PlayOrder) ?? 0;
        
        var queueItem = new QueueItem
        {
            EventId = evt.Id,
            YouTubeVideoId = videoId,
            Title = title,
            Artist = artist,
            DurationSeconds = durationSeconds,
            RequestedBy = requestedBy,
            AddedOn = DateTime.UtcNow,
            PlayOrder = maxOrder + 1,
            IsPlayed = false
        };
        
        context.QueueItems.Add(queueItem);
        await context.SaveChangesAsync();
        
        return queueItem;
    }
    
    public async Task<List<QueueItem>> GetPlaylistForEvent(string eventUid)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = await context.Events
            .FirstOrDefaultAsync(e => e.UniqueId == eventUid);
        
        if (evt == null)
            return new List<QueueItem>();
        
        return await context.QueueItems
            .Where(q => q.EventId == evt.Id)
            .OrderBy(q => q.PlayOrder)
            .ToListAsync();
    }
    
    public async Task<QueueItem?> GetNextSong(string eventUid)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = await context.Events
            .FirstOrDefaultAsync(e => e.UniqueId == eventUid);
        
        if (evt == null)
            return null;
        
        return await context.QueueItems
            .Where(q => q.EventId == evt.Id && !q.IsPlayed)
            .OrderBy(q => q.PlayOrder)
            .FirstOrDefaultAsync();
    }
    
    public async Task MarkAsPlayed(int queueItemId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var item = await context.QueueItems.FindAsync(queueItemId);
        if (item != null)
        {
            item.IsPlayed = true;
            await context.SaveChangesAsync();
        }
    }
    
    public async Task RemoveSong(int queueItemId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var item = await context.QueueItems.FindAsync(queueItemId);
        if (item != null)
        {
            context.QueueItems.Remove(item);
            await context.SaveChangesAsync();
        }
    }
    
    public async Task UpdatePlayOrder(int queueItemId, int newPlayOrder)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var item = await context.QueueItems.FindAsync(queueItemId);
        if (item != null)
        {
            item.PlayOrder = newPlayOrder;
            await context.SaveChangesAsync();
        }
    }
    
    public async Task ResetPlayedStatus(int queueItemId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var item = await context.QueueItems.FindAsync(queueItemId);
        if (item != null)
        {
            item.IsPlayed = false;
            await context.SaveChangesAsync();
        }
    }
    
    public async Task ResetAllPlayedForEvent(string eventUid)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = await context.Events
            .FirstOrDefaultAsync(e => e.UniqueId == eventUid);
        
        if (evt != null)
        {
            var playedSongs = await context.QueueItems
                .Where(q => q.EventId == evt.Id && q.IsPlayed)
                .ToListAsync();
            
            foreach (var song in playedSongs)
            {
                song.IsPlayed = false;
            }
            
            await context.SaveChangesAsync();
        }
    }
    
    public async Task<bool> UpVote(int queueItemId, string voterIdentifier, string ipAddress)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var item = await context.QueueItems.FindAsync(queueItemId);
        if (item == null || item.IsPlayed)
            return false;
        
        // Check if already voted
        var existingVote = await context.VoteRecords
            .FirstOrDefaultAsync(v => v.QueueItemId == queueItemId && v.VoterIdentifier == voterIdentifier);
        
        if (existingVote != null)
            return false; // Already voted
        
        // Record the vote
        var voteRecord = new VoteRecord
        {
            QueueItemId = queueItemId,
            VoterIdentifier = voterIdentifier,
            IsUpVote = true,
            VotedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        };
        
        context.VoteRecords.Add(voteRecord);
        item.UpVotes++;
        await context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> DownVote(int queueItemId, string voterIdentifier, string ipAddress)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var item = await context.QueueItems.FindAsync(queueItemId);
        if (item == null || item.IsPlayed)
            return false;
        
        // Check if already voted
        var existingVote = await context.VoteRecords
            .FirstOrDefaultAsync(v => v.QueueItemId == queueItemId && v.VoterIdentifier == voterIdentifier);
        
        if (existingVote != null)
            return false; // Already voted
        
        // Record the vote
        var voteRecord = new VoteRecord
        {
            QueueItemId = queueItemId,
            VoterIdentifier = voterIdentifier,
            IsUpVote = false,
            VotedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        };
        
        context.VoteRecords.Add(voteRecord);
        item.DownVotes++;
        await context.SaveChangesAsync();
        
        return true;
    }
    
    public async Task<bool> HasVoted(int queueItemId, string voterIdentifier)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        return await context.VoteRecords
            .AnyAsync(v => v.QueueItemId == queueItemId && v.VoterIdentifier == voterIdentifier);
    }
    
    public async Task<Dictionary<int, bool>> GetUserVotes(List<int> queueItemIds, string voterIdentifier)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var votes = await context.VoteRecords
            .Where(v => queueItemIds.Contains(v.QueueItemId) && v.VoterIdentifier == voterIdentifier)
            .ToDictionaryAsync(v => v.QueueItemId, v => v.IsUpVote);
        
        return votes;
    }
}
