using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Shared.Logging;

public static class LoggingExtension
{
    public static void AddLogging(this IHostBuilder host, string serviceName)
    {
        host.UseSerilog((context, configuration) => configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
        );
    }
}