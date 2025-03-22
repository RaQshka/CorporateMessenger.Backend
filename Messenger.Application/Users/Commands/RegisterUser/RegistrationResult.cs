namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegistrationResult
{
    public Guid UserId { get; set; }
    public string EmailConfirmationToken { get; set; }
    public string Message { get; set; }
}