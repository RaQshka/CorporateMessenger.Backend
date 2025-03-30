using MediatR;
using Messenger.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Application.Messages.Commands.CreateMessage
{
    internal class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageResult>
    {
        private readonly IMessageRepository _messageRepository;
        public CreateMessageCommandHandler(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<MessageResult> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {

            return new MessageResult();
        }
    }
}
