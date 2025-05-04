using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class ChatParticipantRepository : IChatParticipantRepository
{
    private readonly MessengerDbContext _context;
    public ChatParticipantRepository(MessengerDbContext context) => _context = context;

    public async Task AddAsync(ChatParticipant participant, CancellationToken ct)
    {
        await _context.ChatParticipants.AddAsync(participant, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        var part = await _context.ChatParticipants
            .FindAsync(new object[] { chatId, userId }, ct);
        if (part != null)
        {
            _context.ChatParticipants.Remove(part);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<List<ChatParticipant>> ListByChatAsync(Guid chatId, CancellationToken ct)
    {
        return await _context.ChatParticipants
            .Include(cp => cp.User)
            .Where(cp => cp.ChatId == chatId)
            .ToListAsync(ct);
    }

    public async Task SetAdminAsync(Guid chatId, Guid userId, bool isAdmin, CancellationToken ct)
    {
        var part = await _context.ChatParticipants
            .FindAsync(new object[] { chatId, userId }, ct);
        if (part != null)
        {
            part.IsAdmin = isAdmin;
            _context.ChatParticipants.Update(part);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExistsAsync(Guid chatId, Guid userId, CancellationToken ct)
    {
        return await _context.ChatParticipants.AnyAsync(x=>
            x.ChatId == chatId && x.UserId == userId);
    }
}