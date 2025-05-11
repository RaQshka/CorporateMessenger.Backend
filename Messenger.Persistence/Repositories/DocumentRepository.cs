using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly MessengerDbContext _context;

    public DocumentRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Document document, CancellationToken ct)
    {
        await _context.Documents.AddAsync(document, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<Document?> GetByIdAsync(Guid documentId, CancellationToken ct)
    {
        return await _context.Documents.Include(x=>x.Chat)
            .FirstOrDefaultAsync(d => d.Id == documentId, ct);
    }

    public async Task<IReadOnlyList<Document>> GetByChatAsync(Guid chatId, CancellationToken ct)
    {
        return await _context.Documents
            .Where(d => d.ChatId == chatId)
            .OrderByDescending(d => d.UploadedAt)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Document document, CancellationToken ct)
    {
        _context.Documents.Update(document);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid documentId, CancellationToken ct)
    {
        var document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentId, ct);
        if (document != null)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync(ct);
        }
    }
}