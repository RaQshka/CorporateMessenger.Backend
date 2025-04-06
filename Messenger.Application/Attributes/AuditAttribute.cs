using Messenger.Domain.Enums;

namespace Messenger.Application.Attributes;

[AttributeUsage( AttributeTargets.Method, AllowMultiple = false)]
public class AuditAttribute : Attribute
{
    public string Description { get; }
    public LogLevel LogLevel { get; }

    public AuditAttribute(string description = "", LogLevel logLevel = LogLevel.Info)
    {
        Description = description;
        LogLevel = logLevel;
    }
}