using Host.Extensions;
using Host.Infrastructure.Caching;
using Host.Infrastructure.Metrics;
using Host.Infrastructure.Notifications;
using Host.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Host.Infrastructure.HttpClients
{
    public class IntegrationClient
    {
        private readonly HttpClient _client;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly OrderInstrumentation _instrumentation;
        private readonly List<RequestInfo> _requests;
        private readonly ILogger<IntegrationClient> _logger;

        private const double expiration = 10 /*minute*/;

        public IntegrationClient(
            HttpClient client,
            OrderInstrumentation instrumentation,
            ILogger<IntegrationClient> logger,
            ICacheService cacheService,
            INotificationService notification,
            IOptions<IntegrationOptions> options)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _instrumentation = instrumentation ?? throw new ArgumentNullException(nameof(instrumentation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _notificationService = notification ?? throw new ArgumentNullException(nameof(notification));
            _requests = options.Value.Requests ?? throw new ArgumentNullException(nameof(options));
        }

        public List<string> GenerateUrls(OrdersModel orders)
        {
            return _requests.Select(request => $"{request.Url}?from={orders.From}&to={orders.To}&time={orders.Time}").ToList();
        }

        public async Task SendAsync(OrdersModel orders, CancellationToken cancellationToken = default)
        {
            int batchSize = 10;
            var batches = GenerateUrls(orders).Batch(batchSize);
            foreach (var urls in batches)
            {
                await ProcessBatchAsync(urls.ToList(), orders, cancellationToken);
            }
        }
        private async Task ProcessBatchAsync(IList<string> urls, OrdersModel model, CancellationToken cancellationToken)
        {
            var tasks = urls.Select(url => ProcessRequestAsync(url, model, cancellationToken));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task ProcessRequestAsync(string url, OrdersModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));

                ArgumentNullException.ThrowIfNull(model);

                using var response = await _client.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken) ?? throw new InvalidOperationException("reason content");

                    var order = JsonSerializer.Deserialize<IReadOnlyList<Order>>(body) ?? throw new InvalidOperationException("reason deserialize");

                    await model.AddAsync(order, _instrumentation, _notificationService, cancellationToken);

                    await _cacheService.SetAsync(model.Id, model, TimeSpan.FromMinutes(expiration), cancellationToken);

                    _logger.LogInformation("response received: " + body);

                    return;
                }
                _logger.LogError($"request to {url} failed with status code {response.StatusCode}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"error sending request to {url}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"error sending request to {url}: {ex.Message}");
            }
        }
    }
}
