using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Interfaces.Services;

/// <summary>
/// Сервис управления правами доступа к чатам по ролям.
/// </summary>
public interface IChatAccessService
{
    /// <summary>
    /// Проверяет, обладает ли пользователь заданным флагом доступа к чату с учетом админов или создателей.
    /// </summary>
    Task<bool> HasAccessAsync(Guid chatId, Guid userId, ChatAccess accessFlag, CancellationToken ct);

    /// <summary>
    /// Задаёт маску доступа для указанной роли в чате.
    /// </summary>
    Task SetMaskAsync(Guid chatId, Guid roleId, int mask, CancellationToken ct);

    /// <summary>
    /// Удаляет правило доступа для роли в чате.
    /// </summary>
    Task RemoveAsync(Guid chatId, Guid roleId, CancellationToken ct);

    /// <summary>
    /// Получает все правила доступа для чата.
    /// </summary>
    Task<IReadOnlyList<ChatAccessRule>> GetAllAsync(Guid chatId, CancellationToken ct);

    /// <summary>
    /// Добавляет права доступа указанной роли (использует OR по маске).
    /// </summary>
    Task GrantAccessAsync(Guid chatId, Guid roleId, ChatAccess rights, CancellationToken cancellationToken);

    /// <summary>
    /// Удаляет определённые права доступа у роли (использует AND NOT по маске).
    /// </summary>
    Task RevokeAccessAsync(Guid chatId, Guid roleId, ChatAccess rights, CancellationToken cancellationToken);
    
    /// <summary>
    /// Проверяет пользователя на наличие прав администратора чата.
    /// </summary>
    Task<bool> IsAdminOfChat(Guid chatId, Guid userId, CancellationToken ct);
}