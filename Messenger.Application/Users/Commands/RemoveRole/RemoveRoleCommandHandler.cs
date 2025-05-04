using MediatR;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Messenger.Application.Users.Commands.RemoveRole;

public class RemoveRoleCommandHandler:IRequestHandler<RemoveRoleCommand,RemoveRoleResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public RemoveRoleCommandHandler( UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }
    
    public async Task<RemoveRoleResult> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return new RemoveRoleResult()
            {
                Success = false,
                Message = "Пользователь не найден."
            };

        if (await _userManager.IsInRoleAsync(user, request.Role))
            return new RemoveRoleResult()
            {
                Success = false,
                Message = "Пользователь уже имеет эту роль."
            };
        

        /*if (_configuration.GetSection("DefaultRoles")
            .GetChildren()
            .All(x => x.Value != request.Role))
        {
            return new RemoveRoleResult()
            {
                Success = false,
                Message = "Несуществующая роль. Измените соответствующие роли в файле конфигурации."
            };
        }      
        */  
        
        await _userManager.RemoveFromRoleAsync(user, request.Role);
        await _userManager.UpdateAsync(user);
        
        return new RemoveRoleResult()
        {
            Success = true,
            Message = "Роль успешно удалена у пользователя."
        };
    }
}