using FluentValidation;

namespace Messenger.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandValidator:AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(command => command.Token).NotEmpty().WithMessage("Token is required");
    }
}