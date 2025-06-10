using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Services;

public interface IEncryptedDocumentService
{
    Task UploadDocumentAsync(Guid chatId, Guid uploaderId, byte[] fileData, byte[] iv, byte[] tag, string fileName, string fileType);
    Task<EncryptedDocument> GetDocumentAsync(Guid documentId, Guid userId);
    Task<IReadOnlyList<EncryptedDocumentDto>> GetDocumentsAsync(Guid chatId, Guid userId, int skip = 0, int take = 100,DateTime? timeStamp = null);

}