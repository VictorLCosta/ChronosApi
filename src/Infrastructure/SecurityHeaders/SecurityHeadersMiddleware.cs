using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.SecurityHeaders;

public class SecurityHeadersMiddleware(IOptions<SecurityHeadersOptions> options) : IMiddleware
{
    private readonly SecurityHeadersOptions _options = options.Value;

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_options.Enabled)
        {
            return next(context);
        }

        var path = context.Request.Path;

        // Allow listed paths (e.g., OpenAPI / Scalar UI) to manage their own scripts/styles.
        if (_options.ExcludedPaths?.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)) == true)
        {
            return next(context);
        }

        var headers = context.Response.Headers;

        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers.XXSSProtection = "0";

        if (!headers.ContainsKey("Content-Security-Policy"))
        {
            var scriptSources = string.Join(' ', _options.ScriptSources ?? []);
            var styleSources = string.Join(' ', _options.StyleSources ?? []);

            var csp =
                "default-src 'self'; " +
                "img-src 'self' data: https:; " +
                $"script-src 'self' https: {scriptSources}; " +
                $"style-src 'self' {(_options.AllowInlineStyles ? "'unsafe-inline' " : string.Empty)}{styleSources}; " +
                "object-src 'none'; " +
                "frame-ancestors 'none'; " +
                "base-uri 'self';";

            headers.ContentSecurityPolicy = csp;
        }

        return next(context);
    }
}