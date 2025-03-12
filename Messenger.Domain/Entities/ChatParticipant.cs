namespace Messenger.Domain;

public class ChatParticipant
{
    public Guid ChatID { get; set; }
    public Chat Chat { get; set; }
    
    public Guid UserID { get; set; }
    public User User { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsAdmin { get; set; }
}