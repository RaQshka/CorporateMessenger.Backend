using FluentValidation;

namespace Messenger.Application.Documents.Commands.RevokeDocumentAccess;

public class RevokeDocumentAccessCommandValidator : AbstractValidator<RevokeDocumentAccessCommand>
{
    public RevokeDocumentAccessCommandValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty().WithMessage("Идентификатор документа обязателен");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Идентификатор роли обязателен");
        RuleFor(x => x.AccessFlag).IsInEnum().WithMessage("Недопустимый флаг доступа");
    }
}