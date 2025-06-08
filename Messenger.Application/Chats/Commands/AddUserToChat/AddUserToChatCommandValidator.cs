using FluentValidation;

namespace Messenger.Application.Chats.Commands.AddUserToChat;

public class AddUserToChatCommandValidator : AbstractValidator<AddUserToChatCommand>
{
    public AddUserToChatCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Идентификатор чата обязателен");

        RuleFor(x => x.UserId)
            //.NotEmpty().WithMessage("Идентификатор пользователя обязателен")
            .NotEqual(x => x.InitiatorId).WithMessage("Нельзя добавить самого себя");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Идентификатор инициатора обязателен");
    }
}