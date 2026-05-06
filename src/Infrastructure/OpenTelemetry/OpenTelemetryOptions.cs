using System.ComponentModel.DataAnnotations;

namespace Infrastructure.OpenTelemetry;

public class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetryOptions";

    public bool Enabled { get; set; }

    internal ExporterOptions Exporter { get; set; } = new();

    internal sealed class ExporterOptions
    {
        public OtlpOptions Otlp { get; set; } = new();
    }

    internal sealed class OtlpOptions
    {
        public bool Enabled { get; set; } = true;

        [Url]
        public string? Endpoint { get; set; }

        /// <summary>
        /// Transport protocol, e.g. "grpc" or "http/protobuf".
        /// </summary>
        public string? Protocol { get; set; }
    }
}