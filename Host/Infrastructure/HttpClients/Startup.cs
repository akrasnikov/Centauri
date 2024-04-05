using Host.Exceptions;

namespace Host.Infrastructure.HttpClients
{
    internal static class Startup
    {
        internal static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration config)
        {
            var storageSettings = config.GetSection("IntegrationOptions").Get<IntegrationOptions>() ?? throw new ExtensionException("Integration is not configured.");
            services.Configure<IntegrationOptions>(config);
            services.AddHttpClient<IntegrationClient>()
                  .AddStandardResilienceHandler(options =>
                  {
                      // Configure standard resilience options here
                  });
            return services;
        }
    }
}