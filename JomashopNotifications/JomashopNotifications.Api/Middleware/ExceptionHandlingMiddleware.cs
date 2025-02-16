using System.Net;
using System.Text.Json;

namespace JomashopNotifications.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next)
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // I should add more exception types here
        var code = HttpStatusCode.InternalServerError;

        context.Response.StatusCode = (int)code;

        var errorDetails = new
        {
            exception.Message,
            context.Response.StatusCode,
            Details = exception.StackTrace
        };

        var result = JsonSerializer.Serialize(errorDetails);

        return context.Response.WriteAsync(result);
    }
}