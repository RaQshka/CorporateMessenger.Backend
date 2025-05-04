namespace Messenger.Application.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string rule)
        : base($"Нарушение бизнес-правила: {rule}")
    {
    }
}