
using MediatR;
using Microsoft.AspNetCore.Identity;
using Messenger.Domain;

namespace Messenger.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, bool>
{
    private readonly UserManager<User> _userManager;

    public ConfirmEmailCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        return result.Succeeded;
    }
}