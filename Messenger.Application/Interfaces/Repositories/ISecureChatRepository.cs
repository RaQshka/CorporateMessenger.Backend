using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface ISecureChatRepository
{
    Task<Domain.Entities.SecureChat> CreateAsync(Domain.Entities.SecureChat chat);
    Task<Domain.Entities.SecureChat> GetByIdAsync(Guid chatId);
    Task<Domain.Entities.SecureChat> GetByAccessKeyAsync(string accessKey);
    Task<IReadOnlyList<Domain.Entities.SecureChat>> GetExpiredChatsAsync();
    Task DeleteAsync(Guid chatId);
}