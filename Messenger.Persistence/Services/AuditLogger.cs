using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Messenger.Persistence.Services;

//TODO: Разделить ответственность логгера: сделать для него отдельный репозиторий и использовать его сервисы.
public class AuditLogger:IAuditLogger
{
    private readonly MessengerDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AuditLogger(MessengerDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(Guid userId, string actionType, string targetEntity, Guid targetId,
        string description, string ipAddress, string userAgent, LogLevel logLevel)
    {
        if (userId == Guid.Empty)
        {
            if (!Guid.TryParse(_httpContextAccessor.HttpContext.User?
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out userId))
            {
                return;
            }
        }
        var log = new AuditLog
        {
            LogId = Guid.NewGuid(),
            UserId = userId,
            ActionType = actionType,
            ActionTime = DateTime.UtcNow,
            TargetEntity = targetEntity,
            TargetId = targetId,
            Description = description,
            IPAddress = ipAddress,
            UserAgent = userAgent,
            LogLevel = logLevel
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
    
    public async Task LogAsync(Guid userId, string actionType, string targetEntity, Guid targetId,
        string description, LogLevel logLevel = LogLevel.Info)
    {
        var context = _httpContextAccessor.HttpContext;

        if (userId == Guid.Empty)
        {
            if (!Guid.TryParse(_httpContextAccessor.HttpContext.User?
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out userId))
            {
                return;
            }
        }
        
        var log = new AuditLog
        {
            LogId = Guid.NewGuid(),
            UserId = userId,
            ActionType = actionType,
            ActionTime = DateTime.UtcNow,
            TargetEntity = targetEntity,
            TargetId = targetId,
            Description = string.IsNullOrEmpty(description)
                ? $"Request to {context.Request.Path}"
                : description,
            IPAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
            LogLevel = logLevel
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
    
}