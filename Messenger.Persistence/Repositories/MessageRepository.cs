using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Repositories;
using Messenger.Application.Messages.Queries.Shared;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
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

    public async Task<IReadOnlyList<MessageDto>> GetByChatAsync(Guid chatId, int skip, int take, CancellationToken ct)
    {
        return await context.Messages
            .Where(m => m.ChatId == chatId && m.IsDeleted == false)
            .OrderByDescending(m => m.SentAt)
            .Include(m => m.Reactions) // Include только для реакций
            .Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                Content = m.Content,
                ReplyToMessageId = m.ReplyToMessageId,
                SentAt = m.SentAt,
                IsDeleted = m.IsDeleted,
                Sender = new SenderInfoDto 
                { 
                    Id = m.SenderId,
                    FirstName = m.Sender.FirstName, 
                    LastName = m.Sender.LastName 
                },
                Reactions = GroupReactionsByType(m.Reactions)
            })
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
    
    // Метод для группировки реакций по типу и подсчета количества
    private static List<MessageReactionsOfTypeDto> GroupReactionsByType(ICollection<MessageReaction> reactions)
    {
        var reactionTypes = new[]
        {
            ReactionType.Like, ReactionType.Heart, ReactionType.Sad, ReactionType.Happy, ReactionType.Cry,
            ReactionType.Laugh
        };
        var groupedReactions = new List<MessageReactionsOfTypeDto>(6); // Максимум 6 типов

        foreach (var type in reactionTypes)
        {
            var count = reactions?.Count(r => r.ReactionType == type) ?? 0;
            if (count > 0)
            {
                groupedReactions.Add(new MessageReactionsOfTypeDto
                {
                    ReactionCount = count,
                    ReactionType = type
                });
            }
        }

        return groupedReactions;
    }
    
}