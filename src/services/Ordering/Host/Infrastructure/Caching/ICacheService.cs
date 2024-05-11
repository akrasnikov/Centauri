namespace Ordering.Host.Infrastructure.Caching;

public interface ICacheService
{
    T? Get<T>(string key);
    Task<T?> GetAsync<T>(string key, CancellationToken token = default);

    Task RefreshAsync(string key, CancellationToken token = default);
    Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
}