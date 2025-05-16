namespace Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;

public class UploadDocumentDto
{
    public Guid ChatId { get; set; }
    public IFormFile File { get; set; }
}

// DTO для предоставления/отзыва доступа