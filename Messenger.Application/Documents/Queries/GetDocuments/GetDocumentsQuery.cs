using MediatR;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.Documents.Queries.GetDocuments;


public class GetDocumentsQuery : IRequest<IReadOnlyList<DocumentDto>>
{
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
}