using MediatR;

namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegisterUserCommand:IRequest<RegistrationResult>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; }= string.Empty;
    public string CorporateKey { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}