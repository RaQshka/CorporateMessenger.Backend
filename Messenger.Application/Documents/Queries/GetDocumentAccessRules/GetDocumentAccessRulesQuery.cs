using MediatR;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Domain.Entities;

namespace Messenger.Application.Documents.Queries.GetDocumentAccessRules;

public class GetDocumentAccessRulesQuery : IRequest<IReadOnlyList<DocumentAccessRuleDto>>
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
}