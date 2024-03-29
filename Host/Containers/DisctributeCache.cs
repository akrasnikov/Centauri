using Host.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Host.Containers
{
    public class DisctributeCache
    {
        /*service:container*/
        private const string key = "containers";
        private const string const_orders = "orders";

        private readonly IDistributedCache _cache;
        private readonly ILogger<DisctributeCache> _logger;

        private readonly TimeSpan const_absolute_expiration = TimeSpan.FromMinutes(4);

        public DisctributeCache(IDistributedCache cache, ILogger<DisctributeCache> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Order>> GetOrderAsync(CancellationToken cancellationToken = default)
        {
            //var json = await _cache.GetStringAsync($"{key}:{const_orders}", cancellationToken);

            //if (string.IsNullOrWhiteSpace(json))
            //{
            //    _logger.LogInformation("PaymentPurpose are not found in redis, starting to sync with database.");

            //    await _cache.SetStringAsync($"{key}:{const_orders}", JsonSerializer.Serialize(collection),

            //        new DistributedCacheEntryOptions
            //        {
            //            AbsoluteExpirationRelativeToNow = const_absolute_expiration
            //        }, cancellationToken);

            //    return collection;
            //}
            return default;

        }
    }
}
