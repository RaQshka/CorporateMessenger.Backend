using Messenger.Domain.Enums;

namespace Messenger.Domain.Entities;

public class MessageReaction
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Message Message { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ReactionType ReactionType { get; set; } // Тип реакции
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}