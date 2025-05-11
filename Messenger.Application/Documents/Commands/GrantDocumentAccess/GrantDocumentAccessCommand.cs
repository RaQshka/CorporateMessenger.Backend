using MediatR;
using Messenger.Domain.Enums;

namespace Messenger.Application.Documents.Commands.GrantDocumentAccess;

public class GrantDocumentAccessCommand : IRequest<Unit>
{
    public Guid DocumentId { get; set; }
    public Guid RoleId { get; set; }
    public DocumentAccess AccessFlag { get; set; }
}