using Microsoft.EntityFrameworkCore;
using PlayPointPlaylist.Models;

namespace PlayPointPlaylist.Data;

public class PlaylistDbContext : DbContext
{
    public PlaylistDbContext(DbContextOptions<PlaylistDbContext> options) : base(options)
    {
    }
    
    public DbSet<Event> Events { get; set; }
    public DbSet<QueueItem> QueueItems { get; set; }
    public DbSet<VoteRecord> VoteRecords { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UniqueId).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UniqueId).IsRequired().HasMaxLength(50);
        });
        
        modelBuilder.Entity<QueueItem>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.YouTubeVideoId).IsRequired().HasMaxLength(20);
            entity.Property(q => q.Title).IsRequired().HasMaxLength(500);
            entity.Property(q => q.Artist).HasMaxLength(200);
            entity.Property(q => q.RequestedBy).HasMaxLength(100);
            
            entity.HasOne(q => q.Event)
                .WithMany(e => e.Playlist)
                .HasForeignKey(q => q.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<VoteRecord>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.VoterIdentifier).IsRequired().HasMaxLength(256);
            entity.Property(v => v.IpAddress).HasMaxLength(45); // IPv6 compatible
            entity.HasIndex(v => new { v.QueueItemId, v.VoterIdentifier }).IsUnique();
            
            entity.HasOne(v => v.QueueItem)
                .WithMany()
                .HasForeignKey(v => v.QueueItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
