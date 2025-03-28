using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Commands.LogoutUser;

public class LogoutUserCommandHandler: IRequestHandler<LogoutUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    
    public LogoutUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
 
        
        return new LogoutUserCommand();
    }
}