using FluentValidation;

namespace Messenger.Application.Chats.Commands.RemoveUserFromChat;

public class RemoveUserFromChatCommandValidator : AbstractValidator<RemoveUserFromChatCommand>
{
    public RemoveUserFromChatCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Идентификатор чата обязателен");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя обязателен");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Идентификатор инициатора обязателен");
    }
}