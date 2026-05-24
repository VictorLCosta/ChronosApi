using CrossCutting.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Web.ExceptionHandlers;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var problemDetails = new ProblemDetails();

        if (exception is FluentValidation.ValidationException fluentException)
        {
            logger.LogWarning(exception, "Validation exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "One or more validation errors occurred.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

            var errors = fluentException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            problemDetails.Extensions.Add("errors", errors);
        }
        else if (exception is UnauthorizedException unauthorizedException)
        {
            logger.LogWarning(unauthorizedException, "Unauthorized access. TraceId: {TraceId}", httpContext.TraceIdentifier);

            problemDetails.Status = StatusCodes.Status401Unauthorized;
            problemDetails.Title = "Unauthorized access.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
            problemDetails.Detail = GetSafeErrorMessage(unauthorizedException, httpContext);

        }
        else
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);

            problemDetails = new ProblemDetails
            {
                Status = 500,
                Title = "An unexpected error occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                Detail = GetSafeErrorMessage(exception, httpContext)
            };
        }

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