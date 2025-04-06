using MediatR;

namespace Messenger.Application.AuditLogs.Queries.ExportUserAuditLogs;

public class ExportUserAuditLogsQuery : IRequest<Stream>
{
    public Guid UserId { get; set; }
    public int? Days { get; set; } = null;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? EndDate { get; set; } = null;
}