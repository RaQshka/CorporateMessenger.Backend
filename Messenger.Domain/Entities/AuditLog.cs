namespace Messenger.Domain;

public class AuditLog
{
    public Guid LogID { get; set; }
    public Guid UserID { get; set; }
    public User User { get; set; }
    
    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;
    public string TargetEntity { get; set; } = string.Empty;
    public Guid TargetID { get; set; }
}