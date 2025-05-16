using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces.Repositories;

public interface IDocumentAccessRepository
{
    Task AddRuleAsync(DocumentAccessRule rule, CancellationToken ct);
    Task<DocumentAccessRule> GetRuleAsync(Guid documentId, string roleName, CancellationToken ct);
    Task<DocumentAccessRule?> GetRuleAsync(Guid documentId, Guid roleId, CancellationToken ct);
    Task<IReadOnlyList<DocumentAccessRule>> GetRulesByDocumentAsync(Guid documentId, CancellationToken ct);
    Task UpdateRuleAsync(DocumentAccessRule rule, CancellationToken ct);
    Task RemoveRuleAsync(Guid documentId, Guid roleId, CancellationToken ct);
}