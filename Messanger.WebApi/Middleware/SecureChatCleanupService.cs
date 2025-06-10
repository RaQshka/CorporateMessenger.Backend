using Messenger.Application.Interfaces.Repositories;

namespace Messenger.WebApi.Middleware;

public class SecureChatCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SecureChatCleanupService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public SecureChatCleanupService(
        IServiceProvider serviceProvider,
        ILogger<SecureChatCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SecureChatCleanupService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredChatsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning up expired chats.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("SecureChatCleanupService stopped.");
    }

    private async Task CleanupExpiredChatsAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var chatRepository = scope.ServiceProvider.GetRequiredService<ISecureChatRepository>();

            var expiredChats = await chatRepository.GetExpiredChatsAsync();
            if (expiredChats == null || !expiredChats.Any())
            {
                _logger.LogDebug("No expired chats found.");
                return;
            }

            foreach (var chat in expiredChats)
            {
                try
                {
                    await chatRepository.DeleteAsync(chat.Id);
                    _logger.LogInformation($"Deleted expired chat with ID {chat.Id}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to delete chat with ID {chat.Id}.");
                }
            }
        }
    }
}