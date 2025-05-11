using MediatR;
using Messenger.Domain.Enums;

namespace Messenger.Application.Documents.Commands.RevokeDocumentAccess;

public class RevokeDocumentAccessCommand : IRequest<Unit>
{
    public Guid DocumentId { get; set; }
    public Guid RoleId { get; set; }
    public DocumentAccess AccessFlag { get; set; }
}