using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Services;

public interface IMessageService
{
    Task<Guid> SendAsync(Guid chatId, Guid senderId, string content, Guid? replyToMessageId, CancellationToken ct);
    Task<IReadOnlyList<Message>> GetByChatAsync(Guid chatId, Guid userId, int skip, int take, CancellationToken ct);
    Task UpdateAsync(Guid messageId, Guid userId, string newContent, CancellationToken ct);
    Task DeleteAsync(Guid messageId, Guid userId, CancellationToken ct);
}