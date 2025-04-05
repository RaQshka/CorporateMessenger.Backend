using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace Messenger.Application.Users.Commands.LogoutUser;

public class LogoutUserCommandHandler: IRequestHandler<LogoutUserCommand, LogoutUserResult>
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
    
    public async Task<LogoutUserResult> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var identity = _httpContext.HttpContext.User.Identity;
        if (identity == null || !identity.IsAuthenticated)
            return new LogoutUserResult()
            {
                Success = false,
                Message = "Выход не удался, не авторизированный доступ."
            }; 
        
        await _signInManager.SignOutAsync();
        // Если используем куки, отправляем пустую куку
        _httpContext.HttpContext.Response.Cookies.Delete("refreshToken");
        _httpContext.HttpContext.Response.Cookies.Delete("AuthToken");
        _httpContext.HttpContext.Response.Cookies.Delete("UserId");
        return new LogoutUserResult()
        {
            Success = true,
            Message = "Вы успешно вышли из системы."
        };
    }
}