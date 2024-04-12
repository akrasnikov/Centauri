using Host.Exceptions;
using Host.Infrastructure.HttpClients;

namespace Host.Infrastructure.Caching;

internal static class Startup
{
    internal static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        services.Configure<CacheSettings>(config);
        if (settings == null) return services;
        if (settings.UseDistributedCache)
        {
            if (settings.PreferRedis)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = settings.ConnectionString;
                    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                    {
                        AbortOnConnectFail = true,
                        EndPoints = { settings?.ConnectionString ?? throw new  InfrastructureException("connectionString is null")}
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            //services.AddTransient<ICacheService, DistributedCacheService>();
        }
        return services;
    }
}