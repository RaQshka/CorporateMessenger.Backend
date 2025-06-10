namespace Messenger.Application.Interfaces.Services;

public interface ISecureChatService
{
    public Task<(string AccessKey, byte[] Salt, byte[] CreatorPublicKey)> CreateChatAsync(
        string name, Guid creatorId, Guid invitedUserId, DateTime destroyAt, byte[] creatorPublicKey);

    public Task<(Guid ChatId, byte[] Salt, byte[] OtherPublicKey)> EnterChatAsync(
        string accessKey, Guid userId, byte[] publicKey);

    Task<bool> IsParticipantAsync(Guid chatId, Guid userId);
}