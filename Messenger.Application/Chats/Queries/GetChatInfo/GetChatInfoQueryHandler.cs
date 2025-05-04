using MediatR;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chats.Queries.GetChatInfo;

public class GetChatInfoQueryHandler : IRequestHandler<GetChatInfoQuery, ChatInfoDto>
{
    private readonly IChatService _chatService;
    private readonly IChatAccessService _accessService;

    public GetChatInfoQueryHandler(IChatService chatService, IChatAccessService accessService)
    {
        _chatService = chatService;
        _accessService = accessService;
    }

    public async Task<ChatInfoDto> Handle(GetChatInfoQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.ReadMessages,
            cancellationToken);

        if (!hasAccess)
            throw new AccessDeniedException("Просмотр информации о чате", request.ChatId, request.InitiatorId);

        var chat = await _chatService.GetByIdAsync(request.ChatId, cancellationToken)
                   ?? throw new NotFoundException("Чат", $"ID: {request.ChatId} не найден");

        return new ChatInfoDto
        {
            Id = chat.Id,
            Name = chat.ChatName,
            Type = chat.ChatType,
            CreatedBy = chat.CreatedBy,
            CreatedAt = chat.CreatedAt
        };
    }
}