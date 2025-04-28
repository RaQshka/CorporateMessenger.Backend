using Messenger.Domain;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Services;

public class ChatRepository : IChatRepository
{
    private readonly MessengerDbContext _context;

    public ChatRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<Chat> CreateChatAsync(Chat chat, CancellationToken cancellationToken)
    {
        await _context.Chats.AddAsync(chat, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return chat;
    }

    public async Task<Chat> GetChatByIdAsync(Guid chatId, CancellationToken cancellationToken)
    {        throw new NotImplementedException();

        
        // Подключаем участников, сообщения и документы для полноценного отображения чата
        return await _context.Chats
            .Include(c => c.ChatParticipants)
            .Include(c => c.Messages.Where(m=>m.SentAt > DateTime.UtcNow.AddDays(30)))
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == chatId, cancellationToken);
    
    }

    public async Task<List<Chat>> GetUserChatsAsync(Guid userId, CancellationToken cancellationToken)
    {        throw new NotImplementedException();

        // Возвращаем чаты, где пользователь является участником
        return await _context.Chats
            .Include(c => c.ChatParticipants)
            .Where(c => c.ChatParticipants.Any(cp => cp.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateChatAsync(Chat chat, CancellationToken cancellationToken)
    {
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteChatAsync(Guid chatId, CancellationToken cancellationToken)
    {
        var chat = await _context.Chats.FindAsync(new object[] { chatId }, cancellationToken);
        if (chat != null)
        {
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
