using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class EncryptedDocumentRepository : IEncryptedDocumentRepository
{
    private readonly MessengerDbContext _context;

    public EncryptedDocumentRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EncryptedDocument document)
    {
        _context.EncryptedDocuments.Add(document);
        await _context.SaveChangesAsync();
    }

    public async Task<EncryptedDocument> GetByIdAsync(Guid documentId)
    {
        return await _context.EncryptedDocuments.FindAsync(documentId);
    }
    
    
    public async Task<IReadOnlyList<EncryptedDocumentDto>> GetByChatAsync(Guid chatId, int skip, int take, DateTime? timeStamp = null)
    {
        return await _context.EncryptedDocuments
            .Where(d => d.SecureChatId == chatId)
            .Select(x=>new EncryptedDocumentDto()
            {
                Timestamp = x.Timestamp,
                Id = x.Id,
                FileName = x.FileName,
                FileSize = x.FileSize,
                FileType = x.FileType,
                UploaderId = x.UploaderId,
                Tag = x.Tag,
                FileData = x.FileData,
                IV = x.IV,
                
                Sender = new SenderInfoDto 
                { 
                    Id = x.UploaderId,
                    FirstName = x.Uploader.FirstName, 
                    LastName = x.Uploader.LastName 
                },
            })
            .OrderByDescending(d => d.Timestamp)
            .Skip(skip).Take(take).Where(x=>x.Timestamp>=timeStamp)
            .ToListAsync();
    }
}