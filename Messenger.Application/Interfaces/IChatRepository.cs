using Messenger.Domain;

public interface IChatRepository
{
    Task<Chat> CreateChatAsync(Chat chat, CancellationToken cancellationToken);
    Task<Chat> GetChatByIdAsync(Guid chatId, CancellationToken cancellationToken);
    Task<List<Chat>> GetUserChatsAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateChatAsync(Chat chat, CancellationToken cancellationToken);
    Task DeleteChatAsync(Guid chatId, CancellationToken cancellationToken);
}