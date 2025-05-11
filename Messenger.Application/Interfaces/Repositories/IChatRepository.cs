using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IChatRepository
{
    Task<Chat> CreateAsync(Chat chat, CancellationToken ct);
    Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<List<Chat>> ListByUserAsync(Guid userId, CancellationToken ct);
    Task UpdateAsync(Chat chat, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}