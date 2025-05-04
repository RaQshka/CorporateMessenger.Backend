namespace Messenger.Application.Common.Exceptions;

public class AccessDeniedException : Exception
{
    public AccessDeniedException(string action, object chatId, object userId)
        : base($"Отказано в доступе для пользователя {userId} при выполнении действия '{action}' в чате {chatId}.")
    {
    }
}