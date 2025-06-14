﻿namespace Messenger.Application.Messages.Queries.Shared;

public class SenderInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}