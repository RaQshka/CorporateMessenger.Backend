using AutoMapper;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.Application.Messages.Commands.Shared;
using Messenger.Domain;

namespace Messenger.Application.Messages.Commands.SendMessage.Messenger.Application.Messages.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IUserAccessService _userAccessService; // Сервис для проверки прав отправки

    public SendMessageCommandHandler(IMessageRepository messageRepository, IMapper mapper, IUserAccessService userAccessService)
    {
        _messageRepository = messageRepository;
        _mapper = mapper;
        _userAccessService = userAccessService;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // Проверка прав отправки сообщения для данного чата
        var canSend = await _userAccessService.CanSendMessageAsync(request.ChatId, request.SenderId);
        if (!canSend)
        {
            throw new UnauthorizedAccessException("У вас нет прав отправлять сообщения в этот чат.");
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            Content = request.Content,
            SentAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var createdMessage = await _messageRepository.SendMessageAsync(message, cancellationToken);
        return _mapper.Map<MessageDto>(createdMessage);
    }
}