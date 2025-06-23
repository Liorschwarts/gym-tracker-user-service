using System.Net;
using System.Text.Json;

namespace GymTracker.UserService.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        object errorResponse = new
        {
            Message = "An error occurred",
            Details = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        response.StatusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            InvalidOperationException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        if (response.StatusCode == 500)
        {
            errorResponse = new { Message = "Internal server error", Timestamp = DateTime.UtcNow };
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(jsonResponse);
    }
}