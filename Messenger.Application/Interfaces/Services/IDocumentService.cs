using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Interfaces.Services;

public interface IDocumentService
{
    Task<Guid> UploadAsync(Guid chatId, Guid uploaderId, IFormFile file, CancellationToken ct);

    Task<(byte[] Content, string FileName, string ContentType)> DownloadAsync(Guid documentId, Guid userId,
        CancellationToken ct);

    Task DeleteAsync(Guid documentId, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Document>> GetListByChatAsync(Guid chatId, Guid userId, CancellationToken ct);
    Task<Document?> GetByIdAsync(Guid documentId, Guid userId, CancellationToken ct);
}