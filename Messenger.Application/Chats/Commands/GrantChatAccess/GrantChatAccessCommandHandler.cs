using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.GrantChatAccess;

public class GrantChatAccessCommandHandler : IRequestHandler<GrantChatAccessCommand, Unit>
{
    private readonly IChatAccessService _accessService;

    public GrantChatAccessCommandHandler(IChatAccessService accessService)
    {
        _accessService = accessService;
    }

    public async Task<Unit> Handle(GrantChatAccessCommand request, CancellationToken cancellationToken)
    {
        bool hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.ManageAccess, // Предполагаемый флаг, нужно добавить
            cancellationToken);

        if (!hasAccess)
        {
            throw new AccessDeniedException("Добавление правил доступа", request.ChatId, request.InitiatorId);
        }
        
        await _accessService.GrantAccessAsync(
            request.ChatId,
            request.RoleId,
            request.Access,
            cancellationToken);

        return Unit.Value;
    }
}