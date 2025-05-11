using FluentValidation;

namespace Messenger.Application.Documents.Queries.DownloadDocument;

public class DownloadDocumentQueryValidator : AbstractValidator<DownloadDocumentQuery>
{
    public DownloadDocumentQueryValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty().WithMessage("Идентификатор документа обязателен");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Идентификатор пользователя обязателен");
    }
}