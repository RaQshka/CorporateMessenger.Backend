using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.RevokeChatAccess;

public class RevokeChatAccessCommandHandler : IRequestHandler<RevokeChatAccessCommand, Unit>
{
    private readonly IChatAccessService _accessService;

    public RevokeChatAccessCommandHandler(
        IChatAccessService accessService)
    {
        _accessService = accessService;
    }

    public async Task<Unit> Handle(RevokeChatAccessCommand request, CancellationToken cancellationToken)
    {
        bool hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.ManageAccess, 
            cancellationToken);

        if (!hasAccess)
        {
            throw new AccessDeniedException("Удаление прав доступа", request.ChatId, request.InitiatorId);
        }
        
        await _accessService.RevokeAccessAsync(
            request.ChatId,
            request.RoleId,
            request.Access,
            cancellationToken);

        return Unit.Value;
    }
}