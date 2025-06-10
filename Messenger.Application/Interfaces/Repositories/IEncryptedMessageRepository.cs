using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IEncryptedMessageRepository
{
    Task AddAsync(EncryptedMessage message);
    Task<List<EncryptedMessageDto>> GetByChatIdAsync(Guid chatId, int skip = 0, int take = 0);
}