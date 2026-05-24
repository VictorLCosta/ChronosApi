using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Web.HealthChecks;

public static class HealthEndpoints
{
    internal sealed record HealthResult(
        string Status,
        DateTimeOffset GeneratedAt,
        double TotalDurationMs,
        int UnhealthyCount,
        int DegradedCount,
        IEnumerable<HealthEntry> Results);

    internal sealed record HealthEntry(
        string Name,
        string Status,
        string? Description,
        double DurationMs,
        string? Error = default,
        Dictionary<string, object?>? Details = default);

    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var group = app.MapGroup("/health")
                       .WithTags("Health")
                       .AllowAnonymous()
                       .DisableRateLimiting();

        group.MapGet("/live", async Task<Ok<HealthResult>> (HealthCheckService hc, IHostEnvironment env, CancellationToken cancellationToken) =>
        {
            var report = await hc.CheckHealthAsync(registration => registration.Tags.Contains("live"), cancellationToken);
            var payload = CreateResponse(report, env);

            return TypedResults.Ok(payload);
        })
        .WithName("Liveness")
        .WithDescription("Reports if the API process is alive.")
        .Produces<HealthResult>(StatusCodes.Status200OK);

        group.MapGet("/ready", async (HealthCheckService hc, IHostEnvironment env, CancellationToken cancellationToken) =>
        {
            var report = await hc.CheckHealthAsync(
                registration => registration.Tags.Contains("ready"),
                cancellationToken);
            var payload = CreateResponse(report, env);
            var statusCode = report.Status switch
            {
                HealthStatus.Healthy => StatusCodes.Status200OK,
                HealthStatus.Degraded => StatusCodes.Status200OK,
                _ => StatusCodes.Status503ServiceUnavailable
            };

            return Results.Json(payload, statusCode: statusCode);
        })
        .WithName("Readiness")
        .WithDescription("Returns dependency readiness and detailed check results with a stable payload shape.")
        .Produces<HealthResult>(StatusCodes.Status200OK)
        .Produces<HealthResult>(StatusCodes.Status503ServiceUnavailable);

        return app;
    }

    private static HealthResult CreateResponse(HealthReport report, IHostEnvironment env)
    {
        var results = report.Entries.Select(entry =>
            new HealthEntry(
                Name: entry.Key,
                Status: entry.Value.Status.ToString(),
                Description: entry.Value.Description,
                DurationMs: entry.Value.Duration.TotalMilliseconds,
                Error: GetSafeError(entry.Value, env),
                Details: entry.Value.Data.Count == 0
                    ? null
                    : entry.Value.Data.ToDictionary(
                        pair => pair.Key,
                        pair => SanitizeDetail(pair.Value))))
            .ToArray();

        return new HealthResult(
            Status: report.Status.ToString(),
            GeneratedAt: DateTimeOffset.UtcNow,
            TotalDurationMs: report.TotalDuration.TotalMilliseconds,
            UnhealthyCount: report.Entries.Count(entry => entry.Value.Status == HealthStatus.Unhealthy),
            DegradedCount: report.Entries.Count(entry => entry.Value.Status == HealthStatus.Degraded),
            Results: results);
    }

    private static string? GetSafeError(HealthReportEntry entry, IHostEnvironment env)
    {
        if (entry.Exception is null)
        {
            return null;
        }

        if (env.IsDevelopment())
        {
            return entry.Exception.Message;
        }

        return entry.Status switch
        {
            HealthStatus.Degraded => "A non-critical dependency is degraded.",
            HealthStatus.Unhealthy => "A critical dependency failed its health check.",
            _ => null
        };
    }

    private static object? SanitizeDetail(object? value)
    {
        if (value is null)
        {
            return null;
        }

        return value switch
        {
            string text => text,
            bool flag => flag,
            byte number => number,
            short number => number,
            int number => number,
            long number => number,
            float number => number,
            double number => number,
            decimal number => number,
            DateTime dateTime => dateTime,
            DateTimeOffset dateTimeOffset => dateTimeOffset,
            TimeSpan timeSpan => timeSpan,
            Enum enumValue => enumValue.ToString(),
            _ => value.ToString()
        };
    }
}