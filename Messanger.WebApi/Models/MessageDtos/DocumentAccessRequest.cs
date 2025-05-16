using Messenger.Domain.Enums;

namespace Messenger.WebApi.Models.MessageDtos.Messenger.Application.DTOs;

public class DocumentAccessRequest
{
    public Guid RoleId { get; set; }
    public DocumentAccess AccessFlag { get; set; }
}