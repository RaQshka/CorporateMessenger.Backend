using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Documents.Commands.RevokeDocumentAccess;

public class RevokeDocumentAccessCommandHandler : IRequestHandler<RevokeDocumentAccessCommand, Unit>
{
    private readonly IDocumentAccessService _documentAccessService;

    public RevokeDocumentAccessCommandHandler(IDocumentAccessService documentAccessService)
    {
        _documentAccessService = documentAccessService;
    }

    public async Task<Unit> Handle(RevokeDocumentAccessCommand request, CancellationToken cancellationToken)
    {
        await _documentAccessService.RevokeAccessAsync(
            request.DocumentId,
            request.InitiatorId,
            request.RoleId,
            request.AccessFlag,
            cancellationToken);
        return Unit.Value;
    }
}