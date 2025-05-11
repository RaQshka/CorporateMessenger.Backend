using MediatR;

namespace Messenger.Application.Documents.Commands.DeleteDocument;

public class DeleteDocumentCommand : IRequest<Unit>
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
}