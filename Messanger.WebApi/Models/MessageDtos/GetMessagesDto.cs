namespace Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;

public class GetMessagesDto
{
    public Guid ChatId { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
}