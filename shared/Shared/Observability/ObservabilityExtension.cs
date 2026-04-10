using Microsoft.Extensions.Hosting;

namespace Shared.Observability;

public static class ObservabilityExtension
{
    public static IHostBuilder AddObservability(this IHostBuilder host, string serviceName)
    {
        host.UseCustomSerilog(serviceName);
        host.ConfigureServices((context, services) =>
        {
            services.AddCustomOpenTelemetry(context.Configuration, context.HostingEnvironment.ApplicationName);
        });

        return host;
    }
}