using Host.Entity;
using Host.Extensions;
using Host.Infrastructure.Caching;
using Host.Infrastructure.Metrics;
using Host.Infrastructure.Notifications;
using Host.Infrastructure.Tracing;
using Host.Infrastructure.Tracing.Aspect;
using Microsoft.Extensions.Options;
using Serilog.Context;
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

        public IEnumerable<IEnumerable<string>> Urls(Orders orders)
        {
            return
                _requests
                .Select(request => $"{request.Url}?from={orders.From}&to={orders.To}&time={orders.Time}")
                .Batch(10);
        }

        [OrderTracingInterceptor(ActivityName = "process-create-request")]
        public async Task SendAsync(Orders orders, CancellationToken cancellationToken = default)
        {
            using var log = LogContext.PushProperty(OrderTracingFactory.CorelationId, orders.Id);

            foreach (var urls in Urls(orders))
            {
                var tasks = urls.Select(url => ProcessRequestAsync(url, orders, cancellationToken));
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            await _cacheService.SetAsync(orders.Id, orders, cancellationToken);
        }


        [OrderTracingInterceptor(ActivityName = "process-send-request")]
        private async Task ProcessRequestAsync(string url, Orders model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));

                ArgumentNullException.ThrowIfNull(model);

                using var request = new HttpRequestMessage(HttpMethod.Get, url);

                request.Headers.TryAddWithoutValidation("X-Correlation-Id", model.Id);

                using var response = await _client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var order =
                        await response
                        .Content
                        .ReadFromJsonAsync<List<Order>>(cancellationToken)
                        ?? throw new InvalidOperationException("reason content");

                    model.Add(order);

                    _logger.LogInformation($"response received -> id: {model.Id} - list : {JsonSerializer.Serialize(order)}");

                    return;
                }

                _logger.LogError($"order id: {model.Id} failed with status code {response.StatusCode}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"order id: {model.Id} for url: {url}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"order id: {model.Id} for url: {url}: {ex.Message}");
            }
        }
    }
}
