using Host.Extensions;
using Host.Infrastructure.Caching;
using Host.Infrastructure.Metrics;
using Host.Infrastructure.Notifications;
using Host.Infrastructure.Tracing;
using Host.Infrastructure.Tracing.Aspect;
using Host.Models;
using Microsoft.Extensions.Options;
using Serilog.Context;
using System.Diagnostics;
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

        public List<string> GenerateUrls(OrdersModel orders)
        {
            return _requests.Select(request => $"{request.Url}?from={orders.From}&to={orders.To}&time={orders.Time}").ToList();
        }

        [OrderTracingInterceptor(ActivityName = "process-create-request")]
        public async Task SendAsync(OrdersModel model, CancellationToken cancellationToken = default)
        {
            using var log = LogContext.PushProperty(OrderTracingFactory.CorelationId, model.Id);
            var batches = GenerateUrls(model).Batch(10);

            _logger.LogInformation($"process batch ");

            foreach (var urls in batches)
            {               
                var tasks = urls.Select(url => ProcessRequestAsync(url, model, cancellationToken));
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        [OrderTracingInterceptor(ActivityName = "process-send-request")]
        private async Task ProcessRequestAsync(string url, OrdersModel model, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));
                ArgumentNullException.ThrowIfNull(model);
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.TryAddWithoutValidation("X-Correlation-Id", model.Id);

                using var response = await _client.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken) ?? throw new InvalidOperationException("reason content");

                    var order = JsonSerializer.Deserialize<List<Order>>(body) ?? throw new InvalidOperationException("reason deserialize");

                    await model.AddAsync(order, _instrumentation, _notificationService, cancellationToken);

                    await _cacheService.SetAsync(model.Id, model, cancellationToken);

                    _logger.LogInformation($"response received -> id: {model.Id} - list : {JsonSerializer.Serialize(order)}");

                    return;
                }

                _logger.LogError($"request to {url} - id: {model.Id} -> failed with status code {response.StatusCode}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"error sending for id: {model.Id} for url:  {url}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"error sending for id: {model.Id} for url: {url}: {ex.Message}");
            }
        }
    }
}
