using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IReactionRepository
{
    Task AddAsync(MessageReaction reaction, CancellationToken ct);
    Task RemoveAsync(Guid reactionId, CancellationToken ct);
    Task<IReadOnlyList<MessageReaction>> GetByMessageAsync(Guid messageId, CancellationToken ct);
}