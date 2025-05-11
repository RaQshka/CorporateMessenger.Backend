using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class ReactionRepository : IReactionRepository
{
    private readonly MessengerDbContext _context;

    public ReactionRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(MessageReaction reaction, CancellationToken ct)
    {
        await _context.MessageReactions.AddAsync(reaction, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(Guid reactionId, CancellationToken ct)
    {
        var reaction = await _context.MessageReactions.FirstOrDefaultAsync(r => r.Id == reactionId, ct);
        if (reaction != null)
        {
            _context.MessageReactions.Remove(reaction);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<IReadOnlyList<MessageReaction>> GetByMessageAsync(Guid messageId, CancellationToken ct)
    {
        return await _context.MessageReactions
            .Where(r => r.MessageId == messageId)
            .Include(r => r.User)
            .ToListAsync(ct);
    }
}