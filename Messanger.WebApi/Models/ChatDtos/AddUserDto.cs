namespace Messenger.WebApi.Models.ChatDtos;

public class AddUserDto
{
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
}