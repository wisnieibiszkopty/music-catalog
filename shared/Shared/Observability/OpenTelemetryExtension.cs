using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Shared.Observability;

public static class OpenTelemetryExtension
{
    public static IServiceCollection AddCustomOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        var otel = services.AddOpenTelemetry();

        otel.ConfigureResource(resource =>
            resource.AddService(serviceName));

        otel.WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter());

        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = httpContext =>
                        !httpContext.Request.Path.Value!.Contains("/metrics") &&
                        !httpContext.Request.Path.Value!.Contains("/health");

                    options.RecordException = true;
                })
                .AddHttpClientInstrumentation()
                .AddSource("MassTransit");

            var endpoint = configuration["Otlp:Endpoint"];
            if (!string.IsNullOrEmpty(endpoint))
            {
                tracing.AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(endpoint);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });

        return services;
    }
}