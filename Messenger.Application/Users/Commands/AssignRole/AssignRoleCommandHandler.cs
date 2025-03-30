using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Commands.AssignRole;

public class IRequestRoleCommandHandler:IRequestHandler<AssignRoleCommand,string>
{
    private readonly UserManager<User> _userManager;

    public IRequestRoleCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    public async Task<string> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return "Пользователь не найден.";

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return "Пользователь уже имеет эту роль.";

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        return result.Succeeded ? "Роль добавлена." : "Ошибка при добавлении роли.";
    }
}