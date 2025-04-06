using System.Text;
using MediatR;
using Messenger.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Application.AuditLogs.Queries.ExportUserAuditLogs;

public class ExportUserAuditLogsQueryHandler : IRequestHandler<ExportUserAuditLogsQuery, Stream>
{
    private readonly IMessengerDbContext _context;

    public ExportUserAuditLogsQueryHandler(IMessengerDbContext context)
    {
        _context = context;
    }

    public async Task<Stream> Handle(ExportUserAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartTime ?? endDate.AddDays(-(request.Days ?? 30));

        var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, new UTF8Encoding(true), leaveOpen: true))
        {
            await writer.WriteLineAsync("\"Log ID\";\"User ID\";\"Action Type\";\"Action Time\";\"Target Entity\";\"Target ID\";\"Description\";\"IP Address\";\"User Agent\";\"Log Level\"");
        
            var logsQuery = _context.AuditLogs
                .Where(l => l.UserId == request.UserId)
                .Where(l => l.ActionTime >= startDate && l.ActionTime <= endDate)
                .OrderByDescending(l => l.ActionTime)
                .AsAsyncEnumerable();
            
            await foreach (var log in logsQuery.WithCancellation(cancellationToken))
            {
                await writer.WriteLineAsync(
                    $"\"{log.LogId}\";" +
                    $"\"{log.ActionTime:yyyy-MM-dd HH:mm:ss}\";" +
                    $"\"{EscapeForExcel(log.ActionType)}\";" +                    
                    $"\"{EscapeForExcel(log.Description)}\";" +
                    $"\"{EscapeForExcel(log.TargetEntity)}\";" +
                    $"\"{log.TargetId}\";" +
                    $"\"{EscapeForExcel(log.IPAddress)}\";" +
                    $"\"{EscapeForExcel(log.UserAgent)}\";" +
                    $"\"{log.LogLevel}\""
                );
            }
        }

        stream.Position = 0;
        return stream;
    }

    private string EscapeForExcel(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
    
        // Экранируем кавычки и заменяем переносы строк (Excel их не любит)
        return value.Replace("\"", "\"\"")
            .Replace("\r", " ")
            .Replace("\n", " ");
    }
}