using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Application.Messages.Commands.CreateMessage
{
    internal class CreateMessageCommand :IRequest<MessageResult>
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }
        public string Message { get;set; }
    }
}
