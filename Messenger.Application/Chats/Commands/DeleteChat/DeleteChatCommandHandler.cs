using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.DeleteChat;

public class DeleteChatCommandHandler : IRequestHandler<DeleteChatCommand, Unit>
{
    private readonly IChatService _chatService;
    private readonly IChatAccessService _accessService;

    public DeleteChatCommandHandler(
        IChatService chatService,
        IChatAccessService accessService)
    {
        _chatService = chatService;
        _accessService = accessService;
    }

    public async Task<Unit> Handle(DeleteChatCommand request, CancellationToken cancellationToken)
    {
        bool hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.DeleteChat,
            cancellationToken);

        if (!hasAccess)
        {
            throw new AccessDeniedException("Удаление чата", request.ChatId, request.InitiatorId);
        }

        await _chatService.DeleteAsync(request.ChatId, cancellationToken);
        return Unit.Value;
    }
}