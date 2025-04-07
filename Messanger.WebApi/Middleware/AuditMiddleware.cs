using System.Security.Claims;
using Messenger.Application.Attributes;
using Messenger.Application.Interfaces;
using LogLevel = Messenger.Domain.Enums.LogLevel;

namespace Messenger.WebApi.Middleware;


public class AuditMiddleware
{
    private readonly RequestDelegate _next;

    public AuditMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogger auditLogger)
    {
        // Проверяем, имеет ли контроллер или действие атрибут Audit
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var auditControllerAttr = endpoint.Metadata.GetMetadata<AuditControllerAttribute>();
            var auditAttr = endpoint.Metadata.GetMetadata<AuditAttribute>();
            var noAuditAttr = endpoint.Metadata.GetMetadata<NoAuditAttribute>();
            
            if (noAuditAttr != null)
            {
                await _next(context);
            }
            
            // Если явно указан NoAudit, пропускаем логирование
            if (auditAttr != null || auditControllerAttr != null)
            {
                // Пример: логирование начала запроса
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var description = string.IsNullOrEmpty(auditAttr?.Description)
                    ? $"Запрос в {context.Request.Path}"
                    : $"Запрос в {context.Request.Path}, сообщение: {auditAttr.Description}";

                // Можно получить IP, User-Agent, и т.д.
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
                
                var parsedUserId = Guid.Empty;
                Guid.TryParse(userId, out parsedUserId);
                
                await auditLogger.LogAsync(
                    parsedUserId, 
                    "StartRequest", 
                    context.Request.Path, 
                    Guid.Empty,
                    description, 
                    ipAddress, 
                    userAgent, 
                    LogLevel.Info);
            }
        }

        await _next(context);

        // Можно также логировать завершение запроса, если требуется
    }
}