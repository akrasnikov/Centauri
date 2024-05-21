using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Exceptions;

namespace Ordering.Infrastructure.HttpClients
{
    internal static class Startup
    {
        internal static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration config)
        {
            var storageSettings = config.GetSection("IntegrationOptions").Get<IntegrationOptions>() ?? throw new ExtensionException("Integration is not configured.");
            services.Configure<IntegrationOptions>(config);
            services.AddHeaderPropagation(options => options.Headers.Add("custom-correlation-id"));
            services.AddHttpClient<IntegrationClient>()
                  .AddStandardResilienceHandler(options =>
                  {
                      // Configure standard resilience options here
                  });
            return services;
        }
    }
}