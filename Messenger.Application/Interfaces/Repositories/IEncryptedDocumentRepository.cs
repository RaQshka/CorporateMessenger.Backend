using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IEncryptedDocumentRepository
{
    Task AddAsync(EncryptedDocument document);
    Task<EncryptedDocument> GetByIdAsync(Guid documentId);
    Task<IReadOnlyList<EncryptedDocumentDto>> GetByChatAsync(Guid chatId, int skip = 0, int take = 0, DateTime? timeStamp = null);

}