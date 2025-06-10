using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Services;

public interface IEncryptedMessageService
{
    Task SendMessageAsync(Guid chatId, Guid senderId, byte[] ciphertext, byte[] iv, byte[] tag);
    Task<List<EncryptedMessageDto>> GetMessagesAsync(Guid chatId, Guid userId, int skip = 0, int take = 100);
}