using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Shared.Observability;

public static class SerilogExtension
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder host, string serviceName)
    {
        host.UseSerilog((context, configuration) => configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithSpan()
            .Enrich.WithProperty("Service", serviceName)
            .WriteTo.Console(new RenderedCompactJsonFormatter())
        );
        
        return host;
    }
}