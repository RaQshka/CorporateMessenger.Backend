using AutoMapper;
using Messenger.Application.Common.Mappings;
using Messenger.Domain;
using Messenger.Domain.Entities;

namespace Messenger.Application.AuditLogs.Queries.Shared;

public class AuditLogDto:IMapWith<AuditLog>
{
    public Guid LogID { get; set; }
    public Guid UserID { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public DateTime ActionTime { get; set; }
    public string TargetEntity { get; set; } = string.Empty;
    public Guid TargetID { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string LogLevel { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<AuditLog, AuditLogDto>();
    }
}