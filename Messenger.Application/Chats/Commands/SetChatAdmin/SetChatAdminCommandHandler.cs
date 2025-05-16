using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.SetChatAdmin;

public class SetChatAdminCommandHandler : IRequestHandler<SetChatAdminCommand, Unit>
{
    private readonly IChatParticipantService _participantService;
    private readonly IChatAccessService _accessService;

    public SetChatAdminCommandHandler(
        IChatParticipantService participantService,
        IChatAccessService accessService)
    {
        _participantService = participantService;
        _accessService = accessService;
    }

    public async Task<Unit> Handle(SetChatAdminCommand request, CancellationToken cancellationToken)
    {

        bool hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.AssignAdmin, 
            cancellationToken);

        if (!hasAccess)
        {                        
            throw new AccessDeniedException("Назначение администратором", request.ChatId, request.InitiatorId);
        }

        await _participantService.SetAdminAsync(
            request.ChatId,
            request.UserId,
            request.IsAdmin,
            cancellationToken);

        return Unit.Value;
    }
}