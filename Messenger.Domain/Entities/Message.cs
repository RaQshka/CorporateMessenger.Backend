namespace Messenger.Domain;

public class Message
{
    public Guid MessageID { get; set; }
    public Guid ChatID { get; set; }
    public Chat Chat { get; set; }
    
    public Guid SenderID { get; set; }
    public User Sender { get; set; }
    
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}