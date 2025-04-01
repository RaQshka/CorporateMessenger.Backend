using FluentValidation;

namespace Messenger.Application.Users.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4).MaximumLength(255);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6).MaximumLength(255);
        RuleFor(x=> x.FirstName).NotEmpty().MinimumLength(2).MaximumLength(255);
        RuleFor(x=> x.LastName).NotEmpty().MinimumLength(1).MaximumLength(255);
        RuleFor(x => x.CorporateKey).NotEmpty();
    }
}