using Messenger.Application.Interfaces;

namespace Messenger.Persistence.Services;

public class MessageRepository:IMessageRepository
{
    public Task SendMessageAsync(Guid chatId, string content)
    {
        throw new NotImplementedException();
    }

    public Task GetGroupMessages(Guid chatId)
    {
        throw new NotImplementedException();
    }
}