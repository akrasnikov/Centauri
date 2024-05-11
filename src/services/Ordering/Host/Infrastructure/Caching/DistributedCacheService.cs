using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Ordering.Host.Common.Interfaces;
using System.Text;

namespace Ordering.Host.Infrastructure.Caching;

#pragma warning disable CA2254
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _expiration;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly ISerializerService _serializer;

    public DistributedCacheService(IDistributedCache cache,
        IOptions<CacheSettings> options, ILogger<DistributedCacheService> logger, ISerializerService serializer)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _expiration = TimeSpan.FromMinutes(10);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public T? Get<T>(string key) =>
        Get(key) is { } data
            ? Deserialize<T>(data)
            : default;

    private byte[]? Get(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            return _cache.Get(key);
        }
        catch
        {
            return null;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
        await GetAsync(key, token) is { } data
            ? Deserialize<T>(data)
            : default;

    private async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        try
        {
            return await _cache.GetAsync(key, token);
        }
        catch
        {
            return null;
        }
    }
    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _cache.RefreshAsync(key, token);
            _logger.LogDebug(string.Format("Cache Refreshed : {0}", key));
        }
        catch
        {
        }
    }


    public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        try
        {
            return SetAsync(key, Serialize(value), _expiration, cancellationToken);
        }
        catch (Exception)
        {

            throw;
        }

    }

    private async Task SetAsync(string key, byte[] value, TimeSpan? expiration = null, CancellationToken token = default)
    {
        try
        {
            await _cache.SetAsync(key, value, GetOptions(_expiration), token);
            _logger.LogDebug($"Added to Cache : {key}");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private byte[] Serialize<T>(T item)
    {
        return Encoding.Default.GetBytes(_serializer.Serialize(item));
    }

    private T Deserialize<T>(byte[] cachedData) =>
        _serializer.Deserialize<T>(Encoding.Default.GetString(cachedData));

    private static DistributedCacheEntryOptions GetOptions(TimeSpan? expiration)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }
        else
        {
            options.SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); // Default expiration time of 10 minutes.
        }
        return options;
    }
}