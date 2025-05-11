using AutoMapper;
using MediatR;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Messages.Queries.Shared;

namespace Messenger.Application.Messages.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IReadOnlyList<MessageDto>>
{
    private readonly IMessageService _messageService;
    private readonly IMapper _mapper;

    public GetMessagesQueryHandler(IMessageService messageService, IMapper mapper)
    {
        _messageService = messageService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageService.GetByChatAsync(
            request.ChatId,
            request.UserId,
            request.Skip,
            request.Take,
            cancellationToken);
        return _mapper.Map<IReadOnlyList<MessageDto>>(messages);
    }
}