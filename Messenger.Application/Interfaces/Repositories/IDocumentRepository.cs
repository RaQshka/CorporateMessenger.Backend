using Messenger.Application.Documents.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task AddAsync(Document document, CancellationToken ct);
    Task<Document?> GetByIdAsync(Guid documentId, CancellationToken ct);
    Task<IReadOnlyList<DocumentDto>> GetByChatAsync(Guid chatId, CancellationToken ct);
    Task UpdateAsync(Document document, CancellationToken ct);
    Task DeleteAsync(Guid documentId, CancellationToken ct);
}