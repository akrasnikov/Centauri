namespace Ordering.Infrastructure.Infrastructure.Caching;

public class CacheSettings
{
    public bool UseDistributedCache { get; set; }
    public bool PreferRedis { get; set; }
    public string? ConnectionString { get; set; }

    public TimeSpan Expiration { get; set; }
}