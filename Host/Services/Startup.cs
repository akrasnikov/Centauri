using Host.Common.Interfaces;
using Host.Common.Services;
using Host.Infrastructure.Metrics;
using Host.Interfaces;

namespace Host.Services;

internal static class Startup
{
    internal static IServiceCollection AddServices(this IServiceCollection services)
    {
        //services.AddTransient<IOrderService, OrderService>();
        //services.AddTransient<ISerializerService, MicrosoftSerializerService>();
       services.AddSingleton<OrderInstrumentation>();

        services.AddServices(typeof(ITransientService), ServiceLifetime.Transient);
        services.AddServices(typeof(IScopedService), ServiceLifetime.Scoped);
        return services;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
    {       

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

    internal static IServiceCollection AddService(this IServiceCollection services, Type serviceType, Type implementationType, ServiceLifetime lifetime) =>
        lifetime switch
        {
            ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
            ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
            ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
            _ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
        };
}