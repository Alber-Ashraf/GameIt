using GameIt.API.Models;
using GameIt.Application.Exeptions;
using System.Net;

namespace GameIt.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        CustomProblemDetails problem = new();

        switch (ex)
        {
            case BadRequestException badRequestException:
                statusCode = HttpStatusCode.BadRequest;
                problem = new CustomProblemDetails
                {
                    Title = badRequestException.Message,
                    Status = (int)statusCode,
                    Detail = badRequestException.InnerException?.Message ?? "Validation failed.",
                    Type = nameof(BadRequestException),
                    Errors = badRequestException.ValidationErrors
                };
                break;

            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                problem = new CustomProblemDetails
                {
                    Title = notFoundException.Message,
                    Status = (int)statusCode,
                    Type = nameof(NotFoundException),
                    Detail = notFoundException.InnerException?.Message ?? "The requested resource was not found."
                };
                break;

            case UnauthorizedAccessException unauthorizedException:
                _logger.LogWarning("Unauthorized access attempt on {Path} (TraceId: {TraceId}): {Message}",
                    httpContext.Request.Path, httpContext.TraceIdentifier, unauthorizedException.Message);
                statusCode = HttpStatusCode.Unauthorized;
                problem = new CustomProblemDetails
                {
                    Title = "Unauthorized",
                    Status = (int)statusCode,
                    Type = nameof(UnauthorizedAccessException),
                    Detail = unauthorizedException.Message
                };
                break;

            default:
                _logger.LogError(ex, "Unhandled server error on {Path} (TraceId: {TraceId}): {Message}",
                    httpContext.Request.Path, httpContext.TraceIdentifier, ex.Message);
                statusCode = HttpStatusCode.InternalServerError;
                problem = new CustomProblemDetails
                {
                    Title = "An error occurred while processing your request.",
                    Status = (int)statusCode,
                    Type = nameof(HttpStatusCode.InternalServerError),
                    Detail = _env.IsDevelopment() ? ex.StackTrace : "An internal server error occurred."
                };
                break;
        }

        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(problem);
    }
}
