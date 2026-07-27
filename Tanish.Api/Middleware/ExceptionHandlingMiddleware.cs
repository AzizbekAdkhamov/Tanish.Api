using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Tanish.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 400,
                Title = "Validation failed",
                Detail = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage))
            });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 404,
                Title = "Not Found",
                Detail = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Request rejected for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 400,
                Title = "Request rejected",
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 500,
                Title = "An unexpected error occurred",
                Detail = ex.Message
            });
        }
    }
}