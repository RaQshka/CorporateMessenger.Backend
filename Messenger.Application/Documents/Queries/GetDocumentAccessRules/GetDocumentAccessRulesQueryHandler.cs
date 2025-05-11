using AutoMapper;
using MediatR;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Documents.Queries.Shared;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;

namespace Messenger.Application.Documents.Queries.GetDocumentAccessRules;

public class GetDocumentAccessRulesQueryHandler : IRequestHandler<GetDocumentAccessRulesQuery, IReadOnlyList<DocumentAccessRuleDto>>
{
    private readonly IDocumentAccessService _documentAccessService;
    private readonly IChatAccessService _chatAccessService;
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    public GetDocumentAccessRulesQueryHandler(
        IDocumentAccessService documentAccessService,
        IChatAccessService chatAccessService,
        IDocumentService documentService,
        IMapper mapper)
    {
        _documentAccessService = documentAccessService;
        _chatAccessService = chatAccessService;
        _documentService = documentService;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DocumentAccessRuleDto>> Handle(GetDocumentAccessRulesQuery request, CancellationToken cancellationToken)
    {
        // Проверка прав администратора
        var document = await _documentService.GetByIdAsync(request.DocumentId, request.UserId, cancellationToken)
                       ?? throw new NotFoundException("Документ", request.DocumentId);
        var isAdmin = await _chatAccessService.IsAdminOfChat(document.ChatId, request.UserId, cancellationToken);
        if (!isAdmin)
            throw new AccessDeniedException("Просмотр правил доступа", document.ChatId, request.UserId);

        var rules = await _documentAccessService.GetRulesAsync(request.DocumentId, cancellationToken);
        return _mapper.Map<IReadOnlyList<DocumentAccessRuleDto>>(rules);
    }
}