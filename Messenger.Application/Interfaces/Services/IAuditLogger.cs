using Messenger.Domain.Enums;

namespace Messenger.Application.Interfaces.Services;

public interface IAuditLogger
{
    Task LogAsync(Guid userId, string actionType, string targetEntity, Guid targetId,
        string description, string ipAddress, string userAgent, LogLevel logLevel = LogLevel.Info);

    Task LogAsync(Guid userId, string actionType, string targetEntity, Guid targetId,
        string description, LogLevel logLevel = LogLevel.Info);
}