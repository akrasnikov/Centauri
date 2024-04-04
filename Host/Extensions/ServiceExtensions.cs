using Autofac;
using Host.Infrastructure.HttpClients;
using Host.Middleware;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Reflection;
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


        #region services
        public static void AddServices(this IServiceCollection services)
        {
            //services.AddTransient<IOrderService, OrderService>();
            //services.AddSingleton<MetricsInstrumentation>();

            services.AddTransient<IDummyService, DummyService>();
            //services
            //.AddServices(typeof(ITransientService), ServiceLifetime.Transient)
            //.AddServices(typeof(IScopedService), ServiceLifetime.Scoped);
        }


        public static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
        {

            var assembly = Assembly.GetExecutingAssembly();


            var interfaceTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(t => interfaceType.IsAssignableFrom(t)
                                && t.IsClass && !t.IsAbstract)
                    .Select(t => new
                    {
                        Service = t.GetInterfaces().FirstOrDefault(),
                        Implementation = t
                    })
                    .Where(t => t.Service is not null
                                && interfaceType.IsAssignableFrom(t.Service));

            foreach (var type in interfaceTypes)
            {
                services.AddService(type.Service!, type.Implementation, lifetime);
            }

            return services;
        }

        public static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime) =>
            lifetime switch
            {
                ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
                ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
                ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
                _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
            };
        #endregion

        
        public static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IntegrationOptions>(configuration);
            services.AddHttpClient<IntegrationClient>()
                  .AddStandardResilienceHandler(options =>
                    {
                        // Configure standard resilience options here
                    }); ;
            return services;
        }
        public static IServiceCollection AddInstrumentation(this IServiceCollection services, IConfiguration configuration)
        {
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

        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services) => services.AddScoped<ErrorHandlerMiddleware>();

        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) => app.UseMiddleware<ErrorHandlerMiddleware>();

        private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");

    }

}
