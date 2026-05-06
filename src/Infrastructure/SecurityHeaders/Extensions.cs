using Microsoft.AspNetCore.Builder;

namespace Infrastructure.SecurityHeaders;

public static class Extensions
{
    public static IApplicationBuilder ConfigureSecurityHeaders(this IApplicationBuilder app)
    {
        app.UseMiddleware<SecurityHeadersMiddleware>();
        return app;
    }

}