using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.RemoveUserFromChat;

public class RemoveUserFromChatCommandHandler : IRequestHandler<RemoveUserFromChatCommand, Unit>
{
    private readonly IChatParticipantService _participantService;
    private readonly IChatAccessService _accessService;

    public RemoveUserFromChatCommandHandler(
        IChatParticipantService participantService,
        IChatAccessService accessService)
    {
        _participantService = participantService;
        _accessService = accessService;
    }

    public async Task<Unit> Handle(RemoveUserFromChatCommand request, CancellationToken cancellationToken)
    {
        //проверка есть ли доступ удалять пользователя у инициатора
        bool hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.RemoveParticipant,
            cancellationToken);

        if (!hasAccess)
        {
            throw new AccessDeniedException("Удаление пользователя из чата", request.ChatId, request.InitiatorId);
        }

        if (request.UserId != Guid.Empty)
        {
        
            await _participantService.RemoveAsync(
                request.ChatId,
                request.UserId,
                cancellationToken);
    
        }
        else if (request.UserEmail != string.Empty)
        {
            await _participantService.RemoveByEmailAsync(
                request.ChatId,
                request.UserEmail,
                cancellationToken);

        }
        return Unit.Value;
    }
}