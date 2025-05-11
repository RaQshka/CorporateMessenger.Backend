using FluentValidation;

namespace Messenger.Application.Messages.Commands.EditMessage.Messenger.Application.Messages.Commands;

public class EditMessageCommandValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty().WithMessage("Идентификатор сообщения обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
        RuleFor(x => x.NewContent)
            .NotEmpty().WithMessage("Новое содержимое сообщения не может быть пустым")
            .MaximumLength(5000).WithMessage("Новое содержимое сообщения не может превышать 5000 символов");
    }
}