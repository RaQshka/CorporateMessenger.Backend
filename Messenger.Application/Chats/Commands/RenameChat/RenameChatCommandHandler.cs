using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.RenameChat;

public class RenameChatCommandHandler : IRequestHandler<RenameChatCommand, Unit>
{
    private readonly IChatService _chatService;
    private readonly IChatAccessService _chatAccessService;
    public RenameChatCommandHandler(
        IChatService chatService,
         IChatAccessService chatAccessService)
    {
        _chatService = chatService;
        _chatAccessService = chatAccessService;
    }

    public async Task<Unit> Handle(RenameChatCommand request, CancellationToken cancellationToken)
    {
        var hasAccess = await _chatAccessService.HasAccessAsync(
            request.ChatId, 
            request.InitiatorId, 
            ChatAccess.RenameChat, 
            cancellationToken);
        
        if (!hasAccess)
        {
            throw new AccessDeniedException("Изменение названия чата", request.ChatId, request.InitiatorId);
        }

        await _chatService.RenameAsync(
            request.ChatId, 
            request.InitiatorId, 
            request.NewName, 
            cancellationToken);
        
        return Unit.Value;
    }
}