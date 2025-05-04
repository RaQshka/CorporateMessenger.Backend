namespace Messenger.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string field, string issue)
        : base($"Ошибка валидации для поля '{field}': {issue}")
    {
    }
}