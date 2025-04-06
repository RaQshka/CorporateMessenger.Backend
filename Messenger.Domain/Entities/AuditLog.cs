using System.ComponentModel.DataAnnotations;
using Messenger.Domain.Enums;

namespace Messenger.Domain;


public class AuditLog
{
    public Guid LogId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }

    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; } = DateTime.UtcNow;
    public string TargetEntity { get; set; } = string.Empty;
    public Guid TargetId { get; set; }

    public string Description { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public LogLevel LogLevel { get; set; } = LogLevel.Info;
}

