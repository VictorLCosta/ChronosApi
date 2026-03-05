using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Infrastructure.Logging;

public static class Extensions
{
    extension(IHostApplicationBuilder app)
    {
        public IHostApplicationBuilder AddAppLogging()
        {
            ArgumentNullException.ThrowIfNull(app);

            app.Services.AddSingleton<HttpRequestContextEnricher>();

            app.Services.AddSerilog((context, logger) =>
            {
                var httpEnricher = context.GetRequiredService<HttpRequestContextEnricher>();

                logger.ReadFrom.Configuration(app.Configuration);

                logger.Enrich.With(httpEnricher);

                logger
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"));

                SetMinimumLogLevel(logger, "Information");
            });

            return app;
        }
    }

    private static void SetMinimumLogLevel(LoggerConfiguration serilogConfig, string minLogLevel)
    {
        switch (minLogLevel.ToLower())
        {
            case "debug":
                serilogConfig.MinimumLevel.Debug();
                break;
            case "information":
                serilogConfig.MinimumLevel.Information();
                break;
            case "warning":
                serilogConfig.MinimumLevel.Warning();
                break;
            default:
                serilogConfig.MinimumLevel.Information();
                break;
        }
    }
}