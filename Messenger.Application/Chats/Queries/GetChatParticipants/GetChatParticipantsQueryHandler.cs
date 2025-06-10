using AutoMapper;
using MediatR;
using Messenger.Application.Chats.Queries.Shared;
using Messenger.Application.Common.Exceptions;
using Messenger.Application.Interfaces;
using Messenger.Application.Interfaces.Services;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Messenger.Application.Chats.Queries.GetChatParticipants;

public class GetChatParticipantsQueryHandler : IRequestHandler<GetChatParticipantsQuery, List<ChatParticipantDto>>
{
    private readonly IChatParticipantService _participantService;
    private readonly IChatAccessService _accessService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    
    public GetChatParticipantsQueryHandler(
        IChatParticipantService participantService,
        IChatAccessService accessService,
        UserManager<User> userManager,
        IMapper mapper)
    {
        _participantService = participantService;
        _accessService = accessService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<List<ChatParticipantDto>> Handle(GetChatParticipantsQuery request, CancellationToken cancellationToken)
    {
        var hasAccess = await _accessService.HasAccessAsync(
            request.ChatId,
            request.InitiatorId,
            ChatAccess.ReadMessages,
            cancellationToken);

        if (!hasAccess)
            throw new AccessDeniedException("Просмотр участников чата", request.ChatId, request.InitiatorId);

        var participants = await _participantService.GetAllAsync(request.ChatId, cancellationToken);
        var participantsDtos = new List<ChatParticipantDto>();

        foreach (var participant in participants)
        {
            var dto = _mapper.Map<ChatParticipantDto>(participant);
            dto.Username = participant.User.UserName;
            dto.FirstName = participant.User.FirstName;
            dto.LastName = participant.User.LastName;
            participantsDtos.Add(dto);
        }
  
        return participantsDtos;
    }
}