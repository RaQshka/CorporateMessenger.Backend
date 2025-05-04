using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Interfaces;

/// <summary>
/// Сервис для управления чатами: создание, удаление, переименование и получение.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Создаёт новый чат с указанным названием, типом и создателем.
    /// Также добавляет создателя как администратора и применяет права по умолчанию.
    /// </summary>
    Task<Chat> CreateAsync(string name, ChatTypes type, Guid creatorId, CancellationToken ct);

    /// <summary>
    /// Возвращает чат по его идентификатору.
    /// </summary>
    Task<Chat?> GetByIdAsync(Guid chatId, CancellationToken ct);

    /// <summary>
    /// Возвращает список всех чатов, в которых участвует пользователь.
    /// </summary>
    Task<IReadOnlyList<Chat>> ListByUserAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Переименовывает чат.
    /// </summary>
    Task RenameAsync(Guid chatId, Guid userId, string newName, CancellationToken ct);

    /// <summary>
    /// Удаляет чат.
    /// </summary>
    Task DeleteAsync(Guid chatId, CancellationToken ct);
}
