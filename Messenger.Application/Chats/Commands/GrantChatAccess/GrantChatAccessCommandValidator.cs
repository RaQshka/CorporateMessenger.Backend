using FluentValidation;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chats.Commands.GrantChatAccess;

public class GrantChatAccessCommandValidator : AbstractValidator<GrantChatAccessCommand>
{
    public GrantChatAccessCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .NotEmpty().WithMessage("Идентификатор чата обязателен");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Идентификатор роли обязателен");

        RuleFor(x => x.Access)
            .NotEmpty().WithMessage("Необходимо указать хотя бы одно право доступа");

        RuleFor(x => x.InitiatorId)
            .NotEmpty().WithMessage("Идентификатор инициатора обязателен");
    }
}