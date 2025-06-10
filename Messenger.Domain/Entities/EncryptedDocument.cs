namespace Messenger.Domain.Entities;

public class EncryptedDocument
{
    public Guid Id { get; set; }
    public Guid SecureChatId { get; set; }
    public Guid UploaderId { get; set; } 
    public byte[] FileData { get; set; } = Array.Empty<byte>(); // Зашифрованные данные
    public byte[] IV { get; set; } = Array.Empty<byte>(); // Вектор инициализации
    public byte[] Tag { get; set; } = Array.Empty<byte>(); // Аутентификационный тег
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public long FileSize { get; set; }

    // Навигационные свойства
    public SecureChat SecureChat { get; set; } = null!;
    public User Uploader { get; set; } = null!;
}