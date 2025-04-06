using MediatR;
using Messenger.Application.Attributes;
using Messenger.Application.AuditLogs.Queries.ExportUserAuditLogs;
using Messenger.Application.AuditLogs.Queries.GetUserAuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Messenger.WebApi.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/logs")]
public class LogsController : BaseController
{
    private readonly IMediator _mediator;

    public LogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Получить список действий пользователя по логам
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="days">Количество дней (по умолчанию 30)</param>
    /// <param name="startTime">Начальная дата</param>
    /// <param name="endDate">Конечная дата</param>
    /// <returns>Список логов</returns>
    [HttpGet("user-audit")]
    [Audit("Получение логов действий пользователя по Id")]
    public async Task<IActionResult> GetAuditLogsAsync(
        [FromQuery] Guid userId,
        [FromQuery] int? days = null,
        [FromQuery] DateTime? startTime = null,
        [FromQuery] DateTime? endDate = null)
    {
        var result = await _mediator.Send(new GetUserAuditLogsQuery
        {
            UserId = userId,
            Days = days,
            StartTime = startTime,
            EndDate = endDate
        });

        return Ok(result);
    }

    /// <summary>
    /// Экспортировать логи пользователя в формате CSV
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <param name="days">Количество дней (по умолчанию 30)</param>
    /// <param name="startTime">Начальная дата</param>
    /// <param name="endDate">Конечная дата</param>
    /// <returns>CSV-файл с логами</returns>
    [HttpGet("export-user-audit")]
    [Audit("Экспорт логов действий пользователя в CSV")]
    public async Task<IActionResult> ExportUserAuditAsync(
        [FromQuery] Guid userId,
        [FromQuery] int? days = null,
        [FromQuery] DateTime? startTime = null,
        [FromQuery] DateTime? endDate = null)
    {
        var stream = await _mediator.Send(new ExportUserAuditLogsQuery
        {
            UserId = userId,
            Days = days,
            StartTime = startTime,
            EndDate = endDate
        });

        return new FileStreamResult(stream, "text/csv")
        {
            FileDownloadName = $"audit_logs_{userId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };
    }
}
