using FluentValidation;

namespace Messenger.Application.Users.Commands.LoginUser;

public class LoginUserCommandValidator:AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(command => command.Password).NotEmpty().WithMessage("Password is required");
    }
}