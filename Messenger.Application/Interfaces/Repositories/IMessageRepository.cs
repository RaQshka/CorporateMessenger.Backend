using Messenger.Application.Messages.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken ct);
    Task<Message?> GetByIdAsync(Guid messageId, CancellationToken ct);
    Task<IReadOnlyList<MessageDto>> GetByChatAsync(Guid chatId, int skip, int take, CancellationToken ct);
    Task UpdateAsync(Message message, CancellationToken ct);
    Task DeleteAsync(Guid messageId, CancellationToken ct);
}