using FluentValidation;

namespace Messenger.Application.Messages.Commands.RemoveReaction;

public class RemoveReactionCommandValidator : AbstractValidator<RemoveReactionCommand>
{
    public RemoveReactionCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty().WithMessage("Идентификатор сообщения обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
    }
}