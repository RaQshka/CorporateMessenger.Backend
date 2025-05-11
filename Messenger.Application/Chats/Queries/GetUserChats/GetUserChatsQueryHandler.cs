using AutoMapper;
using MediatR;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Chats.Queries.GetUserChats;

public class GetUserChatsQueryHandler : IRequestHandler<GetUserChatsQuery, List<UserChatDto>>
{
    private readonly IChatService _chatService;
    private readonly IMapper _mapper;

    public GetUserChatsQueryHandler(IChatService chatService, IMapper mapper)
    {
        _chatService = chatService;
        _mapper = mapper;
    }

    public async Task<List<UserChatDto>> Handle(GetUserChatsQuery request, CancellationToken cancellationToken)
    {
        var chats = await _chatService.ListByUserAsync(request.UserId, cancellationToken);
        
        return _mapper.Map<List<UserChatDto>>(chats);
    }
}