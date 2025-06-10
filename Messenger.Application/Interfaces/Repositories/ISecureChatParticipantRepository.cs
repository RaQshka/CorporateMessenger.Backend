using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface ISecureChatParticipantRepository
{
    Task AddAsync(SecureChatParticipant participant);
    Task<bool> IsParticipantAsync(Guid chatId, Guid userId);
    Task<List<SecureChatParticipant>> GetParticipantsAsync(Guid chatId);
    Task<SecureChatParticipant?> GetByChatAndUserAsync(Guid chatId, Guid userId);
    Task UpdateAsync(SecureChatParticipant participant);
    
}