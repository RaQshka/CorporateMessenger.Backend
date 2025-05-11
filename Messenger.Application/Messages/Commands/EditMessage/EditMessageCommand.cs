using MediatR;
using System;

namespace Messenger.Application.Messages.Commands.EditMessage;

public class EditMessageCommand : IRequest<Unit>
{
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public string NewContent { get; set; }
}