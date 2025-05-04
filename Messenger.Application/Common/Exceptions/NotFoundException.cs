namespace Messenger.Application.Common.Exceptions;

// Обобщенные исключения
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"Сущность \"{entityName}\" с идентификатором {key} не найдена.")
    {
    }
}