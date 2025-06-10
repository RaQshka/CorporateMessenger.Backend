namespace Messenger.Domain.Entities;

public class EncryptedMessage
{
    public Guid Id { get; set; }
    public Guid SecureChatId { get; set; }
    public Guid SenderId { get; set; } 
    public byte[] Content { get; set; } = Array.Empty<byte>(); // Зашифрованный текст
    public byte[] IV { get; set; } = Array.Empty<byte>(); // Вектор инициализации
    public byte[] Tag { get; set; } = Array.Empty<byte>(); // Аутентификационный тег
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public SecureChat SecureChat { get; set; } = null!;
    public User Sender { get; set; } = null!;
}