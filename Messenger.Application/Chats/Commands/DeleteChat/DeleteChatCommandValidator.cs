using FluentValidation;

namespace Messenger.Application.Chats.Commands.DeleteChat;

public class DeleteChatCommandValidator : AbstractValidator<DeleteChatCommand>
{
    public DeleteChatCommandValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty().WithMessage("Id не может быть пустым");
    }
}