using Application.Common.Extensions;

using Microsoft.AspNetCore.Http;

using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Logging;

public class HttpRequestContextEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(propertyFactory);

        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            // Add properties to the log event based on HttpContext
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestMethod", httpContext.Request.Method));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("RequestPath", httpContext.Request.Path));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserAgent", httpContext.Request.Headers.UserAgent));

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.GetUserId();
                var userEmailId = httpContext.User.GetEmail();

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserEmail", userEmailId));
            }
        }
    }
}