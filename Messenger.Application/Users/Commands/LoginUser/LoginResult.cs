namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginResult
{
    public Guid? UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}