namespace Messenger.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }                // Вместо MessageID
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
        
    public Guid SenderId { get; set; }
    public User Sender { get; set; }
        
    public string Content { get; set; } = string.Empty;
    public Guid? ReplyToMessageId { get; set; }  // Добавлено поле для ответа на сообщение
    public Message ReplyToMessage { get; set; }  // Навигационное свойство (опционально)
    public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>(); // Реакции
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}

