using FluentValidation;

namespace Messenger.Application.Chats.Commands.SetChatAdmin;

public class SetChatAdminCommandValidator : AbstractValidator<SetChatAdminCommand>
{
    public SetChatAdminCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Идентификатор чата обязателен");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя обязателен");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Идентификатор инициатора обязателен");
    }
}