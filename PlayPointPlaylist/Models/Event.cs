namespace PlayPointPlaylist.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UniqueId { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    
    // Navigation Property
    public List<QueueItem> Playlist { get; set; } = new();
}
