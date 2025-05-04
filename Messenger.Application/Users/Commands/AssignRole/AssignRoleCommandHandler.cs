using MediatR;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Messenger.Application.Users.Commands.AssignRole;

public class AssignRoleCommandHandler:IRequestHandler<AssignRoleCommand,AssignRoleResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    public AssignRoleCommandHandler(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    public async Task<AssignRoleResult> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return new AssignRoleResult()
            {
                Success = false,
                Message = "Пользователь не найден."
            };

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return new AssignRoleResult()
            {
                Success = false,
                Message = "Пользователь уже имеет эту роль."
            };
        

        if (_configuration.GetSection("DefaultRoles")
            .GetChildren()
            .All(x => x.Value != request.Role))
        {
            return new AssignRoleResult()
            {
                Success = false,
                Message = "Несуществующая роль. Измените соответствующие роли в файле конфигурации."
            };
        }        

        //Удаляем предыдущие роли
        /*var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);*/
        //Добавляем новую роль
        var result = await _userManager.AddToRoleAsync(user, request.Role);
        
        if (!result.Succeeded)
        {
            return new AssignRoleResult()
            {
                Success = false,
                Message = "Ошибка при добавлении роли."
            };
        }
        return new AssignRoleResult()
        {
            Success = true,
            Message = "Роль добавлена."
        };
    }
}