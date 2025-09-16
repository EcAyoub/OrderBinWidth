using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger) => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteProblem(context, ex);
        }
    }

    private static Task WriteProblem(HttpContext context, Exception ex)
    {
        var (status, title) = ex switch
        {
            InvalidOperationException => (HttpStatusCode.Conflict, "Business rule conflict"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid argument"),
            _ => (HttpStatusCode.InternalServerError, "Unexpected error")
        };

        var problem = new
        {
            type = $"https://httpstatuses.io/{(int)status}",
            title,
            status = (int)status,
            traceId = context.TraceIdentifier,
            detail = ex.Message
        };

        var payload = JsonSerializer.Serialize(problem);
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)status;
        return context.Response.WriteAsync(payload);
    }
}
