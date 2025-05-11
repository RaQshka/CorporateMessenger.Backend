using AutoMapper;
using MediatR;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;

namespace Messenger.Application.Documents.Queries.GetDocuments;

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, IReadOnlyList<DocumentDto>>
{
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    public GetDocumentsQueryHandler(IDocumentService documentService, IMapper mapper)
    {
        _documentService = documentService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _documentService.GetListByChatAsync(
            request.ChatId,
            request.UserId,
            cancellationToken);
        return _mapper.Map<IReadOnlyList<DocumentDto>>(documents);
    }
}