using MediatR;

namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginUserCommand:IRequest<AuthResult>
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthResult
{
}