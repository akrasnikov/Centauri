﻿using Autofac;
using Hangfire;
using Hangfire.InMemory;
using Host.Infrastructure.HttpClients;
using Host.Infrastructure.Notifications;
using Host.Middleware;
using Host.Options;
using Host.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using System.Reflection;
using System.Text.Json;

namespace Host.Extensions
{
    public static class ServiceExtensions
    {
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

        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
        {
            //var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
            //if (settings == null) return services;
            //if (settings.UseDistributedCache)
            //{
            //    if (settings.PreferRedis)
            //    {
            //        services.AddStackExchangeRedisCache(options =>
            //        {
            //            options.Configuration = settings.RedisURL;
            //            options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
            //            {
            //                AbortOnConnectFail = true,
            //                EndPoints = { settings.RedisURL }
            //            };
            //        });
            //    }
            //    else
            //    {
            //        services.AddDistributedMemoryCache();
            //    }

            //    services.AddTransient<ICacheService, DistributedCacheService>();
            //}
            return services;
        }
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

        public static IServiceCollection Notifications(this IServiceCollection services, IEndpointRouteBuilder endpoints)
        {
            services.AddSignalR();
            return services;
        }

        public static IEndpointRouteBuilder UseNotifications(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationHub>("/notifications", options =>
            {
                options.CloseOnAuthenticationExpiration = true;
            });
            return endpoints;
        }

        internal static IServiceCollection AddExceptionMiddleware(this IServiceCollection services) => services.AddScoped<ErrorHandlerMiddleware>();

        internal static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) => app.UseMiddleware<ErrorHandlerMiddleware>();

        public static IServiceCollection AddRequestLogging(this IServiceCollection services, IConfiguration config)
        {
            if (GetMiddlewareSettings(config).EnableHttpsLogging)
            {
                services.AddSingleton<RequestLoggingMiddleware>();
                services.AddScoped<ResponseLoggingMiddleware>();
            }

            return services;
        }

        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app, IConfiguration config)
        {
            if (GetMiddlewareSettings(config).EnableHttpsLogging)
            {
                app.UseMiddleware<RequestLoggingMiddleware>();
                app.UseMiddleware<ResponseLoggingMiddleware>();
            }

            return app;
        }

        private static MiddlewareSettings GetMiddlewareSettings(IConfiguration config) =>
       config.GetSection(nameof(MiddlewareSettings)).Get<MiddlewareSettings>()!;

        private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
        endpoints.MapHealthChecks("/api/health");

    }

}
