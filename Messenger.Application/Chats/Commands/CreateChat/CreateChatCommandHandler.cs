using AutoMapper;
using MediatR;
using Messenger.Application.Chats.Commands.Shared;
using Messenger.Domain;

namespace Messenger.Application.Chats.Commands.CreateChat;


public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, ChatDto>
{
    private readonly IChatRepository _chatRepository;
    private readonly IMapper _mapper;

    public CreateChatCommandHandler(IChatRepository chatRepository, IMapper mapper)
    {
        _chatRepository = chatRepository;
        _mapper = mapper;
    }

    public async Task<ChatDto> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            ChatName = request.ChatName,
            ChatType = request.ChatType,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
        };

        chat.ChatParticipants = new List<ChatParticipant>()
        {
            new ()
            {
                ChatId = chat.Id,
                UserId = request.CreatedBy,
                IsAdmin = true
            }
        };
        
        var createdChat = await _chatRepository.CreateChatAsync(chat, cancellationToken);
        return _mapper.Map<ChatDto>(createdChat);
    }
}
