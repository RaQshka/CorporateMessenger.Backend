using FluentValidation;

namespace Messenger.Application.Documents.Queries.GetDocumentAccessRules;

public class GetDocumentAccessRulesQueryValidator : AbstractValidator<GetDocumentAccessRulesQuery>
{
    public GetDocumentAccessRulesQueryValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty().WithMessage("Идентификатор документа обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
    }
}