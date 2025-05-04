using Messenger.Application.Interfaces;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly MessengerDbContext _db;
    public ChatRepository(MessengerDbContext db) => _db = db;

    public async Task<Chat> CreateAsync(Chat chat, CancellationToken ct)
    {
        await _db.Chats.AddAsync(chat, ct);
        await _db.SaveChangesAsync(ct);
        return chat;
    }

    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Chats
            .Include(c => c.ChatParticipants)
            .Include(c => c.Messages)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<List<Chat>> ListByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Chats
            .Where(c => c.ChatParticipants.Any(cp => cp.UserId == userId))
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Chat chat, CancellationToken ct)
    {
        _db.Chats.Update(chat);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var chat = await _db.Chats.FindAsync(new object[] { id }, ct);
        if (chat != null)
        {
            _db.Chats.Remove(chat);
            await _db.SaveChangesAsync(ct);
        }
    }
}