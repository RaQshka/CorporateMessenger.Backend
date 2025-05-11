using MediatR;
using Messenger.Application.Interfaces.Services;

namespace Messenger.Application.Documents.Commands.GrantDocumentAccess;

public class GrantDocumentAccessCommandHandler : IRequestHandler<GrantDocumentAccessCommand, Unit>
{
    private readonly IDocumentAccessService _documentAccessService;

    public GrantDocumentAccessCommandHandler(IDocumentAccessService documentAccessService)
    {
        _documentAccessService = documentAccessService;
    }

    public async Task<Unit> Handle(GrantDocumentAccessCommand request, CancellationToken cancellationToken)
    {
        await _documentAccessService.GrantAccessAsync(
            request.DocumentId,
            request.RoleId,
            request.AccessFlag,
            cancellationToken);
        return Unit.Value;
    }
}