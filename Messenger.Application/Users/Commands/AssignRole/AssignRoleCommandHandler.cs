using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Messenger.Application.Users.Commands.AssignRole;

public class AssignRoleCommandHandler:IRequestHandler<AssignRoleCommand,string>
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    public AssignRoleCommandHandler(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    public async Task<string> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return "Пользователь не найден.";

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return "Пользователь уже имеет эту роль.";

        if (_configuration.GetSection("DefaultRoles")
            .GetChildren()
            .All(x => x.Value != request.Role))
        {
            return "Несуществующая роль. Измените соответствующие роли в файле конфигурации.";
        }        
        
        //Удаляем предыдущие роли
        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);
        //Добавляем новую роль
        var result = await _userManager.AddToRoleAsync(user, request.Role);
        return result.Succeeded ? "Роль добавлена." : "Ошибка при добавлении роли.";
    }
}