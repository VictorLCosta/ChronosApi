using Microsoft.AspNetCore.Builder;

namespace Infrastructure.SecurityHeaders;

public static class Extensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder ConfigureSecurityHeaders()
        {
            app.UseMiddleware<SecurityHeadersMiddleware>();
            return app;
        }
    }
}