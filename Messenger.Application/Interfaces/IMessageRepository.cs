using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Application.Interfaces
{
    public interface IMessageRepository
    {
        public Task SendMessageAsync(Guid chatId, string content);
        public Task GetGroupMessages(Guid chatId);
    }
}
