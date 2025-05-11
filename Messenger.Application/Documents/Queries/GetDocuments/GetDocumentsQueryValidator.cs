using FluentValidation;

namespace Messenger.Application.Documents.Queries.GetDocuments;

public class GetDocumentsQueryValidator : AbstractValidator<GetDocumentsQuery>
{
    public GetDocumentsQueryValidator()
    {
        RuleFor(x => x.ChatId).NotEmpty().WithMessage("Идентификатор чата обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
    }
}