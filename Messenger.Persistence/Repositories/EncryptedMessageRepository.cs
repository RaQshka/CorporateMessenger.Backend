using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Application.SecureChat.Queries;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class EncryptedMessageRepository : IEncryptedMessageRepository
{
    private readonly MessengerDbContext _context;

    public EncryptedMessageRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(EncryptedMessage message)
    {
        _context.EncryptedMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<EncryptedMessageDto>> GetByChatIdAsync(Guid chatId, int skip, int take)
    {
        var query = _context.EncryptedMessages
            .Where(m => m.SecureChatId == chatId) 
            .OrderByDescending(m => m.Timestamp)
            .Select(m => new EncryptedMessageDto()
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Ciphertext = m.Content,
                IV = m.IV,
                Tag = m.Tag,
                SentTimestamp = m.Timestamp,
                Sender = new SenderInfoDto() 
                { 
                    Id = m.SenderId,
                    FirstName = m.Sender.FirstName, 
                    LastName = m.Sender.LastName 
                },
            })
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return await query;
    }
    
}