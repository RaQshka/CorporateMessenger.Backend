﻿using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.AddUserToChat;

public class AddUserToChatCommandHandler : IRequestHandler<AddUserToChatCommand, Unit>
{
    private readonly IChatParticipantService _participantService;
    private readonly IChatAccessService _accessService;
    private readonly UserManager<User> _userManager;

    public AddUserToChatCommandHandler(
        IChatParticipantService participantService,
        IChatAccessService accessService,
        UserManager<User> userManager)
    {
        _participantService = participantService;
        _accessService = accessService;
        _userManager = userManager;
    }

    public async Task<Unit> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
    {
        bool hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.AddParticipant,
            cancellationToken);

        if (!hasAccess)
        {
            throw new AccessDeniedException("Добавление пользователя в чат", request.ChatId, request.InitiatorId);
        }
        
        //по дефолту добавляемый юзер не может быть админом
        if (request.UserId != Guid.Empty)
        {
            await _participantService.AddAsync(
                request.ChatId,
                request.UserId,
                false,
                cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.UserEmail))
        {
            await _participantService.AddByEmailAsync(
                request.ChatId,
                request.UserEmail!,
                false,
                cancellationToken);
        }


        return Unit.Value;
    }
}