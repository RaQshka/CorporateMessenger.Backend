using FluentValidation;

namespace Messenger.Application.Chats.Commands.CreateChat;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(x => x.ChatName)
            .NotEmpty().WithMessage("Имя чата не может быть пустым.");
        RuleFor(x => x.ChatName).NotEmpty().WithMessage("Тип чата не может быть пустым");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Чат нельзя создать без создателя");
    }
}