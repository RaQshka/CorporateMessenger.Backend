using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Messenger.Application.Interfaces;
using Messenger.Domain;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Users.Commands.LoginUser;


public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResult>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider; // Сервис для генерации JWT токенов

    public LoginUserCommandHandler(UserManager<User> userManager, IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Неверный логин или пароль.");
        }
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
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
            UserId = user.Id,
            Token = token,
            Message = "Успешный вход."
        };
    }
}
