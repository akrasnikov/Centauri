using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ordering.Domain.Tracing;

public static class Startup
{
    public static IServiceCollection AddInstrumentation(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenTelemetry()
                .WithTracing(builder => builder
                .AddSource(OrderTracingFactory.ActivityNameOrderSource)
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(OrderTracingFactory.ServiceName))
                .AddHttpClientInstrumentation()
                .AddHangfireInstrumentation()
                .AddAspNetCoreInstrumentation(opt =>
                {
                    opt.RecordException = true;
                    opt.EnrichWithHttpRequest = (activity, httpRequest) =>
                    {
                        var correlationId = httpRequest.HttpContext.TraceIdentifier;
                        activity.SetBaggage("Correlation-Id", correlationId);
                    };
                })
                .AddConsoleExporter()
                .AddOtlpExporter(opts => { opts.Endpoint = new Uri("http://104.131.189.170:4317"); }))
            .WithMetrics(metrics => metrics
                //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("orders.webapi"))
                .AddMeter(OrderInstrumentation.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddPrometheusExporter());
        return services;
    }

}