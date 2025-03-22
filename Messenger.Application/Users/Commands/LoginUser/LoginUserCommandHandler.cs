using Messenger.Application.Interfaces;

namespace Messenger.Application.Users.Commands.LoginUser;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResult>
{
    private readonly IUserService _userService;
    private readonly IJwtProvider _jwtProvider; // Сервис для генерации JWT токенов

    public LoginUserCommandHandler(IUserService userService, IJwtProvider jwtProvider)
    {
        _userService = userService;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.AuthenticateAsync(request.Username, request.Password);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Неверный логин или пароль.");
        }
        if (!user.EmailConfirmed)
        {
            throw new InvalidOperationException("Email не подтверждён.");
        }
        if (user.RegistrationStatus != "Approved")
        {
            throw new InvalidOperationException("Регистрация еще не подтверждена администратором.");
        }

        var token = _jwtProvider.GenerateToken(user);
        return new AuthResult
        {
            UserId = user.UserID,
            Token = token,
            Message = "Успешный вход."
        };
    }
}
