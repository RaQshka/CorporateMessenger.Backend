using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class MessageRepository(MessengerDbContext context) : IMessageRepository
{
    public async Task AddAsync(Message message, CancellationToken ct)
    {
        await context.Messages.AddAsync(message, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Message?> GetByIdAsync(Guid messageId, CancellationToken ct)
    {
        return await context.Messages.FirstOrDefaultAsync(m => m.Id == messageId, ct);
    }

    public async Task<IReadOnlyList<Message>> GetByChatAsync(Guid chatId, int skip, int take, CancellationToken ct)
    {
        return await context.Messages
            .Where(m => m.ChatId == chatId && m.IsDeleted == false)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Message message, CancellationToken ct)
    {
        context.Messages.Update(message);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid messageId, CancellationToken ct)
    {
        var message = await context.Messages.FirstOrDefaultAsync(m => m.Id == messageId, ct);
        if (message != null)
        {
            context.Messages.Remove(message);
            await context.SaveChangesAsync(ct);
        }
    }
}