using Hangfire;
using Hangfire.InMemory;
using Host.Infrastructure.Integrations;
using Host.Interfaces;
using Host.Metrics;
using Host.Middleware;
using Host.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Text.Json;

namespace Host.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IOrderService, OrderService>();
            services.AddSingleton<OrderInstrumentation>();
        }
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Centauri.Host");
            });
        }

        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static void AddControllersExtension(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
                ;
        }

        public static void AddBackgroundService(this IServiceCollection services)
        {
            services.AddHangfire(config => config
                .UseIgnoredAssemblyVersionTypeResolver()
                .UseInMemoryStorage(new InMemoryStorageOptions
                {
                    MaxExpirationTime = TimeSpan.FromHours(6)
                }));

            services.AddHangfireServer(options => options.Queues = new[] { "critical", "default" });
        }

        //Configure CORS to allow any origin, header and method. 
        //Change the CORS policy based on your requirements.
        //More info see: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0

        public static void AddCustomCors(this IServiceCollection services)
        {
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

        public static IServiceCollection AddIntegrationClient(this IServiceCollection services)
        {
            services.AddHttpClient<IntegrationClient>();
            return services;
        }
        public static IServiceCollection AddRateLImitInstrumentation(this IServiceCollection services, IConfiguration configuration)
        {
            var meterName = configuration["OrderInstrumentationMeterName"]?.ToLowerInvariant() ?? throw new NullReferenceException("OrderInstrumentation  meter missing a name");
            services
                .AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("orders.webapi"))
                    .AddMeter("orders.meter")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()                 
                    .AddPrometheusExporter());
            return services;
        }

    }

}
