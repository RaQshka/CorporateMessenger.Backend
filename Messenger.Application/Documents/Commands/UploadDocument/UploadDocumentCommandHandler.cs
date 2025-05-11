using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Documents.Commands.UploadDocument;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Guid>
{
    private readonly IDocumentService _documentService;

    public UploadDocumentCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        return await _documentService.UploadAsync(
            request.ChatId,
            request.UploaderId,
            request.File,
            cancellationToken);
    }
}