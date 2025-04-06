using FluentValidation;

namespace Messenger.Application.AuditLogs.Queries.GetUserAuditLogs;

public class GetUserAuditLogsQueryValidator : AbstractValidator<GetUserAuditLogsQuery>
{
    public GetUserAuditLogsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId не может быть пустым.");
        
        RuleFor(x => x)
            .Must(x => x.Days != null || x.StartTime != null || x.EndDate != null)
            .WithMessage("Должен быть указан хотя бы один параметр фильтрации по дате")
            .When(x => x.Days == null && x.StartTime == null && x.EndDate == null);

    }
}