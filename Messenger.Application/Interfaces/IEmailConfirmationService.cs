namespace Messenger.Application.Interfaces;

public interface IEmailConfirmationService
{
    Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
}