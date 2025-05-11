using FluentValidation;

namespace Messenger.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator:AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.UserId).NotEmpty().WithMessage("UserId is required");
    }
}