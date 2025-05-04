using Messenger.Domain.Enums;

namespace Messenger.WebApi.Models.ChatDtos;

public class AccessDto
{
    public Guid RoleId { get; set; }
    public ChatAccess Access { get; set; }
}