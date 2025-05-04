using Messenger.Domain;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface IChatParticipantRepository
{
    Task AddAsync(ChatParticipant participant, CancellationToken ct);
    Task RemoveAsync(Guid chatId, Guid userId, CancellationToken ct);
    Task<List<ChatParticipant>> ListByChatAsync(Guid chatId, CancellationToken ct);
    Task SetAdminAsync(Guid chatId, Guid userId, bool isAdmin, CancellationToken ct);
    Task<bool> ExistsAsync(Guid chatId, Guid userId, CancellationToken ct);
}
