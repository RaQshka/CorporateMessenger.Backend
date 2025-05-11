using AutoMapper;
using MediatR;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Messages.Queries.Shared;

namespace Messenger.Application.Messages.Queries.GetReactions;

public class GetReactionsQueryHandler : IRequestHandler<GetReactionsQuery, IReadOnlyList<MessageReactionDto>>
{
    private readonly IReactionService _reactionService;
    private readonly IMapper _mapper;

    public GetReactionsQueryHandler(IReactionService reactionService, IMapper mapper)
    {
        _reactionService = reactionService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MessageReactionDto>> Handle(GetReactionsQuery request, CancellationToken cancellationToken)
    {
        var reactions = await _reactionService.GetByMessageAsync(
            request.MessageId,
            request.UserId,
            cancellationToken);
        return _mapper.Map<IReadOnlyList<MessageReactionDto>>(reactions);
    }
}