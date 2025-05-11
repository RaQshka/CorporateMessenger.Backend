using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Interfaces.Services;

public interface IReactionService
{
    Task AddAsync(Guid messageId, Guid userId, ReactionType reactionType, CancellationToken ct);
    Task RemoveAsync(Guid messageId, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<MessageReaction>> GetByMessageAsync(Guid messageId, Guid userId, CancellationToken ct);
}