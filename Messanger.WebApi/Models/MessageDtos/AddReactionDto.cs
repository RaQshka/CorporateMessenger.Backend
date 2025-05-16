namespace Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;

public class AddReactionDto
{
    public Guid MessageId { get; set; }
    public int ReactionType { get; set; }
}