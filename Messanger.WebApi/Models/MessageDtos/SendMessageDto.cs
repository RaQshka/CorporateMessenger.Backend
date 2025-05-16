namespace Messenger.WebApi.Models.MessageDtos;


// Для SendMessageCommand
public class SendMessageDto
{
    public Guid ChatId { get; set; }
    public string Content { get; set; }
    public Guid? ReplyToMessageId { get; set; }
}