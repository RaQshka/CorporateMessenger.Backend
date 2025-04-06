using MediatR;
using Messenger.Application.AuditLogs.Queries.Shared;

namespace Messenger.Application.AuditLogs.Queries.GetUserAuditLogs;

public class GetUserAuditLogsQuery:IRequest<List<AuditLogDto>>
{
    public Guid UserId { get; set; }
    public int? Days { get; set; } = null;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}