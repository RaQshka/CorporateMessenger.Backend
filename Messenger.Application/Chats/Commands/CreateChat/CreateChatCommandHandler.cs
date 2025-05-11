using AutoMapper;
using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain;
using Messenger.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Commands.CreateChat;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Guid>
{
    private readonly IChatService _chatService;

    public CreateChatCommandHandler(IChatService chatService, UserManager<User> userManager)
    {
        _chatService = chatService;
    }

    public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = await _chatService.CreateAsync(
            request.Name,
            request.Type,
            request.CreatorId,
            cancellationToken);

        return chat.Id;
    }
}