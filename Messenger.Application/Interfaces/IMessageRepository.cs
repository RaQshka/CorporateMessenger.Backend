using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Domain;

namespace Messenger.Application.Interfaces;

public interface IMessageRepository
{
    Task<Message> SendMessageAsync(Message message, CancellationToken cancellationToken);
    Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken);
    Task UpdateMessageAsync(Message message, CancellationToken cancellationToken);
    Task DeleteMessageAsync(Guid messageId, CancellationToken cancellationToken);
}
