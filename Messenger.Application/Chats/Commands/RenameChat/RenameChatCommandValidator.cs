using System.Text.RegularExpressions;
using FluentValidation;

namespace Messenger.Application.Chats.Commands.RenameChat;

public class RenameChatCommandValidator : AbstractValidator<RenameChatCommand>
{
    public RenameChatCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Идентификатор чата обязателен");

        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage("Новое имя чата обязательно")
            .MaximumLength(100).WithMessage("Имя чата не должно превышать 100 символов")
            .Matches(new Regex(@"^[a-zA-Zа-яА-Я0-9\s\-_]+$"))
            .WithMessage("Имя чата может содержать только буквы, цифры, пробелы, дефисы и подчеркивания");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Идентификатор инициатора обязателен");
    }
}