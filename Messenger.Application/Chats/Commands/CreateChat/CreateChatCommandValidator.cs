using System.Text.RegularExpressions;
using FluentValidation;

namespace Messenger.Application.Chats.Commands.CreateChat;

public class CreateChatCommandValidator : AbstractValidator<CreateChatCommand>
{
    public CreateChatCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя чата обязательно")
            .MaximumLength(200).WithMessage("Имя чата не должно превышать 200 символов")
            .Matches(new Regex(@"^[a-zA-Zа-яА-Я0-9\s\-_]+$"))
            .WithMessage("Имя чата может содержать только буквы, цифры, пробелы, дефисы и подчеркивания");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Недопустимый тип чата");

        RuleFor(x => x.CreatorId)
            .NotEmpty().WithMessage("Идентификатор создателя обязателен");
    }
}