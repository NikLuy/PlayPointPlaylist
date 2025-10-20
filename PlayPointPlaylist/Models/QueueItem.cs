namespace PlayPointPlaylist.Models;

public class QueueItem
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string YouTubeVideoId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime AddedOn { get; set; }
    public int PlayOrder { get; set; }
    public bool IsPlayed { get; set; }
    public int UpVotes { get; set; } = 0;
    public int DownVotes { get; set; } = 0;
    
    // Navigation Property
    public Event? Event { get; set; }
}
