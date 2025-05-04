using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.SetChatAdmin;

public class SetChatAdminCommandHandler : IRequestHandler<SetChatAdminCommand, Unit>
{
    private readonly IChatParticipantService _participantService;
    private readonly IChatAccessService _accessService;
    private readonly IChatService _chatService;
    private readonly UserManager<User> _userManager;

    public SetChatAdminCommandHandler(
        IChatParticipantService participantService,
        IChatAccessService accessService,
        IChatService chatService,
        UserManager<User> userManager)
    {
        _participantService = participantService;
        _accessService = accessService;
        _chatService = chatService;
        _userManager = userManager;
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