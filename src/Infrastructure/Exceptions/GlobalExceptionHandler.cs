using Application.Common.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

        var problemDetails = new ProblemDetails();
        problemDetails.Instance = httpContext.Request.Path;

        if (exception is FluentValidation.ValidationException fluentException)
        {
            problemDetails.Title = "one or more validation errors occurred.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            List<string> validationErrors = [];
            foreach (var error in fluentException.Errors)
            {
                validationErrors.Add(error.ErrorMessage);
            }
            problemDetails.Extensions.Add("errors", validationErrors);
        }
        else
        {
            problemDetails = new ProblemDetails
            {
                Status = 500,
                Title = "An unexpected error occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Instance = httpContext.Request.Path,
                Detail = GetSafeErrorMessage(exception, httpContext)
            };
        }

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static string? GetSafeErrorMessage(Exception exception, HttpContext context)
    {
        // Only expose details in development
        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            return exception.Message;
        }

        // In production, only expose messages from our own exceptions
        return exception is AppException ? exception.Message : null;
    }
}