
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Services;
/// <summary>
/// Сервис управления участниками чатов.
/// </summary>
public interface IChatParticipantService
{
    /// <summary>
    /// Добавляет пользователя в чат как участника (опционально администратора).
    /// </summary>
    Task AddAsync(Guid chatId, Guid userId, bool isAdmin, CancellationToken ct);
    /// <summary>
    /// Добавляет пользователя в чат как участника по его почте (опционально администратора).
    /// </summary>
    Task AddByEmailAsync(Guid chatId, string userEmail, bool isAdmin, CancellationToken ct);

    /// <summary>
    /// Удаляет пользователя из чата.
    /// </summary>
    Task RemoveAsync(Guid chatId, Guid userId, CancellationToken ct);

    /// <summary>
    /// Назначает или снимает роль администратора у участника чата.
    /// </summary>
    Task SetAdminAsync(Guid chatId, Guid userId, bool isAdmin, CancellationToken ct);

    /// <summary>
    /// Возвращает список идентификаторов всех участников чата.
    /// </summary>
    Task<IReadOnlyList<ChatParticipant>> GetAllAsync(Guid chatId, CancellationToken ct);
}