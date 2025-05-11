using FluentValidation;

namespace Messenger.Application.Messages.Queries.GetMessages;

public class GetMessagesQueryValidator : AbstractValidator<GetMessagesQuery>
{
    public GetMessagesQueryValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty().WithMessage("Идентификатор чата обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
        RuleFor(x => x.Skip).GreaterThanOrEqualTo(0).WithMessage("Пропуск должен быть неотрицательным");
        RuleFor(x => x.Take).InclusiveBetween(1, 100).WithMessage("Количество сообщений должно быть от 1 до 100");
    }
}