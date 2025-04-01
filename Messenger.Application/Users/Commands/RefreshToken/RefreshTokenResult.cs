namespace Messenger.Application.Users.Commands.RefreshToken;

public class RefreshTokenResult
{
    public bool Success { get; set; }
    public string AccessToken { get; set; }
    public string Message { get; set; }

}