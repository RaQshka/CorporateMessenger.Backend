using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Messenger.Application.Users.Commands.LogoutUser;

public class LogoutUserCommandHandler: IRequestHandler<LogoutUserCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContext;
    
    public LogoutUserCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor httpContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;       
        _httpContext = httpContext;
    }
    
    public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        // Если используем куки, отправляем пустую куку
        _httpContext.HttpContext.Response.Cookies.Delete("jwt");
    }
}