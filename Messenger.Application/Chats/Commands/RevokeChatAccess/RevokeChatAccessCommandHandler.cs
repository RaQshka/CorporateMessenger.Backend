using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.RevokeChatAccess;

public class RevokeChatAccessCommandHandler : IRequestHandler<RevokeChatAccessCommand, Unit>
{
    private readonly IChatAccessService _accessService;
    private readonly IChatService _chatService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public RevokeChatAccessCommandHandler(
        IChatAccessService accessService,
        IChatService chatService,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _accessService = accessService;
        _chatService = chatService;
        _userManager = userManager;
        _roleManager = roleManager;
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