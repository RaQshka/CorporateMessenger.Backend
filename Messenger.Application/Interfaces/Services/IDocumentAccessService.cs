using Messenger.Domain.Entities;
using Messenger.Domain.Enums;

namespace Messenger.Application.Interfaces.Services;

public interface IDocumentAccessService
{
    Task GrantAccessAsync(Guid documentId, Guid initiatorId, Guid roleId, DocumentAccess accessFlag, CancellationToken ct);
    Task RevokeAccessAsync(Guid documentId, Guid initiatorId, Guid roleId, DocumentAccess accessFlag, CancellationToken ct);
    Task<bool> HasAccessAsync(Guid documentId, Guid userId, DocumentAccess accessFlag, CancellationToken ct);
    Task<IReadOnlyList<DocumentAccessRule>> GetRulesAsync(Guid documentId, CancellationToken ct);
}
