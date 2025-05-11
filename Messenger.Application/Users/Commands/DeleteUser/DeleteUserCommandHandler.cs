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
                   ?? throw new NotFoundException("User", request.UserId);

        // Remove associated ChatParticipants
        var participants = await _context.ChatParticipants
            .Where(cp => cp.UserId == request.UserId)
            .ToListAsync(cancellationToken);
        _context.ChatParticipants.RemoveRange(participants);

        // Save changes to remove ChatParticipants
        await _context.SaveChangesAsync(cancellationToken);

        // Delete the user
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new BusinessRuleException("Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return Unit.Value;
    }
}