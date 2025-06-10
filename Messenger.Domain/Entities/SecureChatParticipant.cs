namespace Messenger.Domain.Entities;

public class SecureChatParticipant
{
    public Guid Id { get; set; }
    public Guid SecureChatId { get; set; }
    public Guid UserId { get; set; }
    
    public byte[]? PublicKey { get; set; } // поле для хранения публичного ключа
    // Навигационные свойства
    public SecureChat SecureChat { get; set; } = null!;
    public User User { get; set; } = null!;
}