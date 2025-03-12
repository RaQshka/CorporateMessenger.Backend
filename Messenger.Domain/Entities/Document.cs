namespace Messenger.Domain;

public class Document
{
    public Guid DocumentID { get; set; }
    public Guid ChatID { get; set; }
    public Chat Chat { get; set; }
    
    public Guid UploaderID { get; set; }
    public User Uploader { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty; // Хранение пути к файлу
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}