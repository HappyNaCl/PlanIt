using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlanIt.Domain.Common.Exceptions;
using PlanIt.Domain.Common.Exceptions.Validation;

namespace PlanIt.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var appException = exception as ApiException;
        var statusCode = (int)(appException?.StatusCode ?? HttpStatusCode.InternalServerError);

        var errors = exception is ValidationException validationException
            ? validationException.Errors
            : new Dictionary<string, string[]> { { "general", [exception.Message] } };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(new
        {
            data = (object?)null,
            error = errors
        }, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

        await context.Response.WriteAsync(json);
    }
}