using FluentValidation;

namespace Messenger.Application.Messages.Queries.GetReactions;

public class GetReactionsQueryValidator : AbstractValidator<GetReactionsQuery>
{
    public GetReactionsQueryValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty().WithMessage("Идентификатор сообщения обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
    }
}