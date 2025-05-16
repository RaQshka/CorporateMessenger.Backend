using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Application.Users.Commands.DeleteUser;
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly UserManager<User> _userManager;
    private readonly IMessengerDbContext _context;

    public DeleteUserCommandHandler(UserManager<User> userManager, IMessengerDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                   ?? throw new NotFoundException("Пользователь", request.UserId);

        // Удаление ChatParticipants
        var participants = await _context.ChatParticipants
            .Where(cp => cp.UserId == request.UserId)
            .ToListAsync(cancellationToken);
        _context.ChatParticipants.RemoveRange(participants);

        // Удаление Documents
        var documents = await _context.Documents
            .Where(d => d.UploaderId == request.UserId)
            .ToListAsync(cancellationToken);
        _context.Documents.RemoveRange(documents); // DocumentAccessRules удалятся каскадно

        // Удаление MessageReactions
        var reactions = await _context.MessageReactions
            .Where(r => r.UserId == request.UserId)
            .ToListAsync(cancellationToken);
        _context.MessageReactions.RemoveRange(reactions);

        // Пометка сообщений как удаленных
        var messages = await _context.Messages
            .Where(m => m.SenderId == request.UserId)
            .ToListAsync(cancellationToken);
        foreach (var message in messages)
        {
            message.IsDeleted = true;
        }

        // Сохранение изменений
        await _context.SaveChangesAsync(cancellationToken);

        // Удаление пользователя
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new BusinessRuleException("Не удалось удалить пользователя: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Unit.Value;
    }
}