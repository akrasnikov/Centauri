using Autofac;
using Host.Infrastructure.HttpClients;
using Host.Infrastructure.Metrics;
using Host.Infrastructure.Tracing;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.Json;

namespace Host.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddControllersExtension(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.WriteIndented = true;
                })
                ;
        }

        public static void AddCustomCors(this IServiceCollection services, IContainer container)
        {
            //container.Register(i => new DummyLogger());
            //builder.RegisterType<DummyService>().As<IDummyService>().EnableInterfaceInterceptors().InterceptedBy(typeof(DummyLogger));
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
        }

        public static IServiceCollection AddInstrumentation(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOpenTelemetry()
                    .WithTracing(builder => builder
                    .AddSource(ActivityProvider.ActivityName)
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddOtlpExporter())
                .WithMetrics(metrics => metrics
                    //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("orders.webapi"))
                    .AddMeter(OrderInstrumentation.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter());
            return services;
        }

        private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");

    }

}
