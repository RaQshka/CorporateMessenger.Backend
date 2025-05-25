using MediatR;
using Messenger.Application.Messages.Commands.SendMessage;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.WebApi.Hubs;

public class ChatHub : Hub
{
    private readonly IMediator _mediator;

    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Метод для отправки сообщения
    public async Task SendMessage(string chatId, string content, string replyToMessageId)
    {
        // Создаем команду для отправки сообщения
        var command = new SendMessageCommand
        {
            ChatId = Guid.Parse(chatId),
            SenderId = Guid.Parse(Context.UserIdentifier),
            Content = content,
            ReplyToMessageId = string.IsNullOrEmpty(replyToMessageId) ? null : Guid.Parse(replyToMessageId)
        };

        // Отправляем команду через MediatR
        var messageId = await _mediator.Send(command);

        // Уведомляем всех клиентов о новом сообщении
        await Clients.All.SendAsync("ReceiveMessage", chatId, Context.UserIdentifier, content, messageId);
    }

    // Метод для уведомления о загрузке документа
    public async Task DocumentUploaded(string chatId, string documentId)
    {
        // Уведомляем всех клиентов о новом документе
        await Clients.All.SendAsync("ReceiveDocument", chatId, documentId);
    }
}