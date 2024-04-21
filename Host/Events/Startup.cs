using Host.Events.Consumers;
using MassTransit;
using System.Reflection;

namespace Host.Events;

internal static class Startup
{
    public static IServiceCollection AddEvents(this IServiceCollection services)
    {

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            var entryAssembly = Assembly.GetEntryAssembly();

            x.AddConsumers(entryAssembly);
            
            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    } 
}