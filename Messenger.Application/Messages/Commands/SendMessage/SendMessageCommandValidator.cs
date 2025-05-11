using FluentValidation;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty().WithMessage("Идентификатор чата обязателен");
        RuleFor(x => x.SenderId).NotEmpty().WithMessage("Идентификатор отправителя обязателен");
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Содержимое сообщения не может быть пустым")
            .MaximumLength(5000).WithMessage("Содержимое сообщения не может превышать 5000 символов");
        RuleFor(x => x.ReplyToMessageId).NotEqual(Guid.Empty).When(x => x.ReplyToMessageId.HasValue)
            .WithMessage("Некорректный идентификатор сообщения для ответа");
    }
}