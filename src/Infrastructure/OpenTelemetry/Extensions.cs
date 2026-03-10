using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Npgsql;

using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using static Infrastructure.OpenTelemetry.OpenTelemetryOptions;

namespace Infrastructure.OpenTelemetry;

public static class Extensions
{
    extension(IHostApplicationBuilder builder)
    {
        public IHostApplicationBuilder ConfigureOpenTelemetry()
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.AddOptions<OpenTelemetryOptions>()
                .BindConfiguration(SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var options = new OpenTelemetryOptions();
            builder.Configuration.GetSection(SectionName).Bind(options);

            if (options.Enabled)
            {
                return builder;
            }

            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(serviceName: builder.Environment.ApplicationName);

            builder.Services.AddSingleton(new ActivitySource(builder.Environment.ApplicationName));

            ConfigureMetricsAndTracing(builder, options, resourceBuilder);

            return builder;
        }
    }

    private static void ConfigureMetricsAndTracing(
        IHostApplicationBuilder builder,
        OpenTelemetryOptions options,
        ResourceBuilder resourceBuilder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(builder.Environment.ApplicationName))
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddNpgsqlInstrumentation();

                if (options.Exporter.Otlp.Enabled)
                {
                    metrics.AddOtlpExporter(otlp =>
                    {
                        ConfigureOtlpExporter(options.Exporter.Otlp, otlp);
                    });
                }
            })
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation(instrumentation =>
                    {
                        instrumentation.EnrichWithHttpRequest = EnrichWithHttpRequest;
                        instrumentation.EnrichWithHttpResponse = EnrichWithHttpResponse;
                    })
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(builder.Environment.ApplicationName);

                if (options.Exporter.Otlp.Enabled)
                {
                    tracing.AddOtlpExporter(otlp =>
                    {
                        ConfigureOtlpExporter(options.Exporter.Otlp, otlp);
                    });
                }
            });
    }

    private static void EnrichWithHttpRequest(Activity activity, HttpRequest request)
    {
        activity.SetTag("http.method", request.Method);
        activity.SetTag("http.scheme", request.Scheme);
        activity.SetTag("http.host", request.Host.Value);
        activity.SetTag("http.target", request.Path);
    }

    private static void EnrichWithHttpResponse(Activity activity, HttpResponse response)
    {
        activity.SetTag("http.status_code", response.StatusCode);
    }

    private static void ConfigureOtlpExporter(
        OtlpOptions options,
        OtlpExporterOptions otlp)
    {
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            otlp.Endpoint = new Uri(options.Endpoint);
        }

        var protocol = options.Protocol?.Trim().ToLowerInvariant();
        otlp.Protocol = protocol switch
        {
            "grpc" => OtlpExportProtocol.Grpc,
            "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => otlp.Protocol
        };
    }
}