namespace Messenger.Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }
    
    public Guid UploaderId { get; set; }
    public User Uploader { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public Guid? MessageId { get; set; } 
    public Message Message { get; set; } 
}