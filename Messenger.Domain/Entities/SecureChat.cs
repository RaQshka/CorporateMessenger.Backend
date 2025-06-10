namespace Messenger.Domain.Entities;

public class SecureChat
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DestroyAt { get; set; }
    public string AccessKey { get; set; } = string.Empty; 
    public byte[] Salt { get; set; } = Array.Empty<byte>(); 
    
    public Guid CreatorId { get; set; } 

    // Навигационное свойств
    public ICollection<SecureChatParticipant> Participants { get; set; } = new List<SecureChatParticipant>();
    public ICollection<EncryptedMessage> Messages { get; set; } = new List<EncryptedMessage>();
    public ICollection<EncryptedDocument> Documents { get; set; } = new List<EncryptedDocument>();
}