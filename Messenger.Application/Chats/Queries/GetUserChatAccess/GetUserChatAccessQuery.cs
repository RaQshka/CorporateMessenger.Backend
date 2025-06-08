using AutoMapper;
using MediatR;
using Messenger.Application.Chats.Queries.GetChatAccessRules;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Enums;

namespace Messenger.Application.Chats.Queries.GetUserChatAccess;

public class GetUserChatAccessQuery : IRequest<int>
{
    public Guid ChatId { get; set; }
    public Guid InitiatorId { get; set; }
}


public class GetUserChatAccessQueryHandler : IRequestHandler<GetUserChatAccessQuery, int>
{
    private readonly IChatAccessService _accessService;
    private readonly IMapper _mapper;

    public GetUserChatAccessQueryHandler(
        IChatAccessService accessService,
        IMapper mapper)
    {
        _accessService = accessService;
        _mapper = mapper;
    }

    public async Task<int> Handle(GetUserChatAccessQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await _accessService.GetMaskAsync(
            request.ChatId,
            request.InitiatorId,
            cancellationToken);

        return hasAccess;
    }
}