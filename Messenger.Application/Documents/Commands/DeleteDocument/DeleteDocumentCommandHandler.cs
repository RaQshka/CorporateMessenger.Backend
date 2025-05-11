using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
{
    private readonly IDocumentService _documentService;

    public DeleteDocumentCommandHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        await _documentService.DeleteAsync(
            request.DocumentId,
            request.UserId,
            cancellationToken);
        return Unit.Value;
    }
}