using MediatR;

namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginUserCommand:IRequest<LoginResult>
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; }= string.Empty;
}