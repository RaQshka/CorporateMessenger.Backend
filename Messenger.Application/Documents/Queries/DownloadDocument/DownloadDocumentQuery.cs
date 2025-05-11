using MediatR;
using Messenger.Application.Documents.Queries.Shared;

namespace Messenger.Application.Documents.Queries.DownloadDocument;

public class DownloadDocumentQuery : IRequest<DocumentDownloadDto>
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
}