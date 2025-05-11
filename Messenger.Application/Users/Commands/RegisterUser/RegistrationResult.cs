namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegistrationResult
{
    public Guid UserId { get; set; }
    public string EmailConfirmationToken { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}