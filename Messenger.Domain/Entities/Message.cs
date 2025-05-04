namespace Messenger.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }                // Вместо MessageID
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
        
    public Guid SenderId { get; set; }
    public User Sender { get; set; }
        
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}