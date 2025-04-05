using FluentValidation;

namespace Messenger.Application.Users.Commands.ConfirmAccount;

public class ConfirmAccountCommandValidator : AbstractValidator<ConfirmAccountCommand>
{
    public ConfirmAccountCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty().WithMessage("User Id is required");
    }
}