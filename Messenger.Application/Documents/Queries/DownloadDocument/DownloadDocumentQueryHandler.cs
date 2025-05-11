using AutoMapper;
using MediatR;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Documents.Queries.DownloadDocument;

public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, DocumentDownloadDto>
{
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    public DownloadDocumentQueryHandler(IDocumentService documentService, IMapper mapper)
    {
        _documentService = documentService;
        _mapper = mapper;
    }

    public async Task<DocumentDownloadDto> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        var result = await _documentService.DownloadAsync(
            request.DocumentId,
            request.UserId,
            cancellationToken);
        return _mapper.Map<DocumentDownloadDto>(result);
    }
}