using FluentValidation;

namespace Messenger.Application.Documents.Commands.GrantDocumentAccess;

public class GrantDocumentAccessCommandValidator : AbstractValidator<GrantDocumentAccessCommand>
{
    public GrantDocumentAccessCommandValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty().WithMessage("Идентификатор документа обязателен");
        RuleFor(x => x.InitiatorId).NotEmpty().WithMessage("Идентификатор инициатора обязателен");
        RuleFor(x => x.RoleId).NotEmpty().WithMessage("Идентификатор роли обязателен");
        RuleFor(x => x.AccessFlag).IsInEnum().WithMessage("Недопустимый флаг доступа");
    }
}