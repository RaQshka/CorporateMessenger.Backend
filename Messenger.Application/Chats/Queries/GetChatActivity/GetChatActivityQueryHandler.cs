using AutoMapper;
using MediatR;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Application.Interfaces.Services;
using Messenger.Application.Messages.Queries.Shared;

namespace Messenger.Application.Chats.Queries.GetChatActivity;

public class GetChatActivityQueryHandler : IRequestHandler<GetChatActivityQuery, IReadOnlyList<ChatActivityDto>>
{
    private readonly IMessageService _messageService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    public GetChatActivityQueryHandler(IMessageService messageService, IDocumentService documentService, IMapper mapper)
    {
        _messageService = messageService;
        _documentService = documentService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ChatActivityDto>> Handle(GetChatActivityQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageService.GetByChatAsync(request.ChatId, request.UserId, 0, request.Take, cancellationToken);
        var documents = await _documentService.GetListByChatAsync(request.ChatId, request.UserId, cancellationToken);

        var activity = messages.Select(m => new ChatActivityDto
            {
                Id = m.Id,
                Type = "Message",
                Timestamp = m.SentAt,
                Data = _mapper.Map<MessageDto>(m)
            }).Concat(documents.Select(d => new ChatActivityDto
            {
                Id = d.Id,
                Type = "Document",
                Timestamp = d.UploadedAt,
                Data = _mapper.Map<DocumentDto>(d)
            })).OrderByDescending(a => a.Timestamp)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();

        return activity;
    }
}