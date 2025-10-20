namespace PlayPointPlaylist.Models;

public class VoteRecord
{
    public int Id { get; set; }
    public int QueueItemId { get; set; }
    public string VoterIdentifier { get; set; } = string.Empty; // IP + Fingerprint Hash
    public bool IsUpVote { get; set; }
    public DateTime VotedAt { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    
    // Navigation Property
    public QueueItem? QueueItem { get; set; }
}
