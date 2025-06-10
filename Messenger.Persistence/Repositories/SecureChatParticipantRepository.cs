using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class SecureChatParticipantRepository : ISecureChatParticipantRepository
{
    private readonly MessengerDbContext _context;

    public SecureChatParticipantRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SecureChatParticipant participant)
    {
        _context.SecureChatParticipants.Add(participant);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsParticipantAsync(Guid chatId, Guid userId)
    {
        return await _context.SecureChatParticipants.AnyAsync(p => p.SecureChatId == chatId && p.UserId == userId);
    }

    public async Task<List<SecureChatParticipant>> GetParticipantsAsync(Guid chatId)
    {
        return await _context.SecureChatParticipants
            .Where(p => p.SecureChatId == chatId)
            .ToListAsync();
    }

    public async Task<SecureChatParticipant?> GetByChatAndUserAsync(Guid chatId, Guid userId)
    {
        return await _context.SecureChatParticipants
            .FirstOrDefaultAsync(x => 
                x.SecureChatId == chatId && 
                x.UserId == userId);
    }

    public async Task UpdateAsync(SecureChatParticipant participant)
    {
        _context.SecureChatParticipants.Update(participant);
        await _context.SaveChangesAsync();
    }
}