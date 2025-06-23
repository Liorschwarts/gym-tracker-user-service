using System.Net;
using System.Text.Json;
using GymTracker.UserService.Exceptions;

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
            Message = GetErrorMessage(exception),
            Timestamp = DateTime.UtcNow
        };

        response.StatusCode = GetStatusCode(exception);

        if (response.StatusCode == 500)
        {
            errorResponse = new { Message = "Internal server error", Timestamp = DateTime.UtcNow };
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        UserAlreadyExistsException => (int)HttpStatusCode.Conflict,
        UserNotFoundException => (int)HttpStatusCode.NotFound,
        InvalidCredentialsException => (int)HttpStatusCode.Unauthorized,
        ArgumentException => (int)HttpStatusCode.BadRequest,
        UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
        KeyNotFoundException => (int)HttpStatusCode.NotFound,
        _ => (int)HttpStatusCode.InternalServerError
    };

    private static string GetErrorMessage(Exception exception) => exception switch
    {
        UserAlreadyExistsException or UserNotFoundException or InvalidCredentialsException => exception.Message,
        ArgumentException => exception.Message,
        _ => "An error occurred"
    };
}