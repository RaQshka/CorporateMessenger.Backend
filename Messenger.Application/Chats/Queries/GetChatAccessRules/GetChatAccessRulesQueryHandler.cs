using AutoMapper;
using MediatR;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Queries.GetChatAccessRules;

public class GetChatAccessRulesQueryHandler : IRequestHandler<GetChatAccessRulesQuery, List<ChatAccessRuleDto>>
{
    private readonly IChatAccessService _accessService;
    private readonly IMapper _mapper;

    public GetChatAccessRulesQueryHandler(
        IChatAccessService accessService,
        IMapper mapper)
    {
        _accessService = accessService;
        _mapper = mapper;
    }

    public async Task<List<ChatAccessRuleDto>> Handle(GetChatAccessRulesQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.ManageAccess, // Предполагается, что для просмотра правил нужен ManageAccess
            cancellationToken);

        if (!hasAccess)
            throw new AccessDeniedException("Просмотр правил доступа", request.ChatId, request.InitiatorId);

        var rules = await _accessService.GetAllAsync(request.ChatId, cancellationToken);
        var result = new List<ChatAccessRuleDto>();

        foreach (var rule in rules)
        {
            var chatAccessRule = _mapper.Map<ChatAccessRuleDto>(rule);
            result.Add(chatAccessRule);
        }

        return result;
    }
}