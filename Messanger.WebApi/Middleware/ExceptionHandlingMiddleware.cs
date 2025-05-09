using System.Net;
using System.Text.Json;
using Messenger.Application.Common.Exceptions;

namespace Messenger.WebApi.Middleware;

// Middleware для обработки исключений
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case NotFoundException notFound:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new { error = notFound.Message });
                break;
            case AccessDeniedException accessDenied:
                code = HttpStatusCode.Forbidden;
                result = JsonSerializer.Serialize(new { error = accessDenied.Message });
                break;
            case ValidationException validation:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new { error = validation.Message });
                break;
            case BusinessRuleException businessRule:
                code = HttpStatusCode.Conflict;
                result = JsonSerializer.Serialize(new { error = businessRule.Message });
                break;
            case FluentValidation.ValidationException fluentValidation:
                code = HttpStatusCode.BadRequest;
                result =JsonSerializer.Serialize(new { error = fluentValidation.Errors.Select(e => e.ErrorMessage)});
                break;
            default:
                result = JsonSerializer.Serialize(new { error = $"Произошла непредвиденная ошибка: {exception.Message}" });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}