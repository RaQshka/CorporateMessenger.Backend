using Messenger.Application.Interfaces;
using Messenger.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Services;

public class MessageRepository : IMessageRepository
{
    private readonly MessengerDbContext _context;

    public MessageRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<Message> SendMessageAsync(Message message, CancellationToken cancellationToken)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken)
    {
        // Сортировка по дате отправления (по возрастанию)
        return await _context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateMessageAsync(Message message, CancellationToken cancellationToken)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteMessageAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var message = await _context.Messages.FindAsync(new object[] { messageId }, cancellationToken);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
