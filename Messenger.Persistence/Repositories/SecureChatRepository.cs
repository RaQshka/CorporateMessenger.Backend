using Messenger.Application.Interfaces.Repositories;
using Messenger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Persistence.Repositories;

public class SecureChatRepository : ISecureChatRepository
{
    private readonly MessengerDbContext _context;

    public SecureChatRepository(MessengerDbContext context)
    {
        _context = context;
    }

    public async Task<SecureChat> CreateAsync(SecureChat chat)
    {
        _context.SecureChats.Add(chat);
        await _context.SaveChangesAsync();
        return chat;
    }

    public async Task<SecureChat> GetByIdAsync(Guid chatId)
    {
        return await _context.SecureChats.FindAsync(chatId);
    }

    public async Task<SecureChat> GetByAccessKeyAsync(string accessKey)
    {
        return await _context.SecureChats.FirstOrDefaultAsync(sc => sc.AccessKey == accessKey);
    }

    public async Task<IReadOnlyList<SecureChat>> GetExpiredChatsAsync()
    {
        return await _context.SecureChats.Where(x=>x.DestroyAt < DateTime.Now).ToListAsync();
    }

    public async Task DeleteAsync(Guid chatId)
    {
        var chat = await _context.SecureChats.FindAsync(chatId);
        if (chat != null)
        {
            _context.SecureChats.Remove(chat);
            await _context.SaveChangesAsync();
        }
    }
}