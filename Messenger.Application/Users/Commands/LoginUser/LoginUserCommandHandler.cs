using System.Security.Claims;
using System.Text;
using MediatR;
using Messenger.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;
using Messenger.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResult>
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(SignInManager<User> signInManager, UserManager<User> userManager, IJwtProvider jwtProvider)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<LoginResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            return new LoginResult { Message = "Неверные учетные данные." };

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (!result.Succeeded)
            return new LoginResult { Message = "Ошибка входа." };

        if (!user.EmailConfirmed)
            return new LoginResult { Message = "Email не подтверждён." };

        if (user.RegistrationStatus != "Approved")
            return new LoginResult { Message = "Регистрация не одобрена администратором." };

        var token = await _jwtProvider.GenerateToken(user);

        return new LoginResult
        {
            UserId = user.Id,
            Token = token,
            Message = "Успешный вход."
        };
    }
}