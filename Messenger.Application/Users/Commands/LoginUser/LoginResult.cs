namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginResult
{
    public Guid? UserId { get; set; }
    public string Token { get; set; }
    public string Message { get; set; }
}