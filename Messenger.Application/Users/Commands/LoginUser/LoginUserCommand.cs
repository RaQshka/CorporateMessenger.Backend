using MediatR;

namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginUserCommand:IRequest<AuthResult>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; }= string.Empty;
}