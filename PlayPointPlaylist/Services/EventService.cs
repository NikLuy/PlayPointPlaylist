using Microsoft.EntityFrameworkCore;
using PlayPointPlaylist.Data;
using PlayPointPlaylist.Models;

namespace PlayPointPlaylist.Services;

public class EventService
{
    private readonly IDbContextFactory<PlaylistDbContext> _contextFactory;
    
    public EventService(IDbContextFactory<PlaylistDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    public async Task<Event> CreateEvent(string name)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = new Event
        {
            Name = name,
            UniqueId = GenerateUniqueId(),
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };
        
        context.Events.Add(evt);
        await context.SaveChangesAsync();
        
        return evt;
    }
    
    public async Task<List<Event>> GetAllEvents()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Include(e => e.Playlist)
            .OrderByDescending(e => e.CreatedOn)
            .ToListAsync();
    }
    
    public async Task<Event?> GetEventByUniqueId(string uniqueId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Events
            .Include(e => e.Playlist)
            .FirstOrDefaultAsync(e => e.UniqueId == uniqueId);
    }
    
    public async Task ToggleEventStatus(int eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = await context.Events.FindAsync(eventId);
        if (evt != null)
        {
            evt.IsActive = !evt.IsActive;
            await context.SaveChangesAsync();
        }
    }
    
    public async Task DeleteEvent(int eventId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        
        var evt = await context.Events.FindAsync(eventId);
        if (evt != null)
        {
            context.Events.Remove(evt);
            await context.SaveChangesAsync();
        }
    }
    
    private string GenerateUniqueId()
    {
        // Generate a short, unique ID (6 characters)
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
