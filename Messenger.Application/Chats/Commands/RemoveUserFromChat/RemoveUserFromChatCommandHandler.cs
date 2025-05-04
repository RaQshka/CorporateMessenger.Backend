using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
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

        await _participantService.RemoveAsync(
            request.ChatId,
            request.UserId,
            cancellationToken);

        return Unit.Value;
    }
}