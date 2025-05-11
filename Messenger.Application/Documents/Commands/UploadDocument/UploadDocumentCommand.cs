using MediatR;
using Microsoft.AspNetCore.Http;

namespace Messenger.Application.Documents.Commands.UploadDocument;

public class UploadDocumentCommand : IRequest<Guid>
{
    public Guid ChatId { get; set; }
    public Guid UploaderId { get; set; }
    public IFormFile File { get; set; }
}