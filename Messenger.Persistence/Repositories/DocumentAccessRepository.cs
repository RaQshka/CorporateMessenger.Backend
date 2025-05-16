using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;
public class DocumentAccessRepository : IDocumentAccessRepository
{
    private readonly MessengerDbContext _context;

    public DocumentAccessRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task AddRuleAsync(DocumentAccessRule rule, CancellationToken ct)
    {
        await _context.DocumentAccessRules.AddAsync(rule, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<DocumentAccessRule?> GetRuleAsync(Guid documentId, Guid roleId, CancellationToken ct)
    {
        return await _context.DocumentAccessRules
            .FirstOrDefaultAsync(r => r.DocumentId == documentId && r.RoleId == roleId, ct);
    }

    public async Task<DocumentAccessRule?> GetRuleAsync(Guid documentId, string roleName, CancellationToken ct)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName, ct);
        if (role == null)
            throw new ArgumentException($"Роль {roleName} не найдена");

        return await _context.DocumentAccessRules
            .FirstOrDefaultAsync(r => r.DocumentId == documentId && r.RoleId == role.Id, ct);
    }

    public async Task<IReadOnlyList<DocumentAccessRule>> GetRulesByDocumentAsync(Guid documentId, CancellationToken ct)
    {
        return await _context.DocumentAccessRules
            .Include(r => r.Role)
            .Where(r => r.DocumentId == documentId)
            .ToListAsync(ct);
    }

    public async Task UpdateRuleAsync(DocumentAccessRule rule, CancellationToken ct)
    {
        _context.DocumentAccessRules.Update(rule);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveRuleAsync(Guid documentId, Guid roleId, CancellationToken ct)
    {
        var rule = await GetRuleAsync(documentId, roleId, ct);
        if (rule != null)
        {
            _context.DocumentAccessRules.Remove(rule);
            await _context.SaveChangesAsync(ct);
        }
    }
}