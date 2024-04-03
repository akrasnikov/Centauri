using Host.Caching;
using Host.Entities;
using Host.Exceptions;
using Host.Infrastructure.Notifications;
using Host.Metrics;
using Host.Models;
using Host.Options;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Linq;
using Host.Extensions;

namespace Host.Infrastructure.Integrations
{
    public class IntegrationClient
    {
        private readonly HttpClient _client;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly MetricsInstrumentation _instrumentation;       
        private readonly List<RequestInfo> _requests;
        private readonly ILogger<IntegrationClient> _logger;
        
        private const double expiration = 1 /*minute*/;

        public IntegrationClient(
            HttpClient client,
            MetricsInstrumentation instrumentation,
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

        public List<string> RequestFromOptions(OrdersModel orders)
        {
            return _requests.Select(request => $"{request.Url}?from={orders.From}&to={orders.To}&time={orders.Time}").ToList();
        }

        public async Task SendAsync(OrdersModel orders, CancellationToken cancellationToken = default)
        {
            int batchSize = 10;
            var urls = RequestFromOptions(orders);

            int totalBatches = (int)Math.Ceiling((double)urls.Count / batchSize);

            var batches = urls.Batch(batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var batchs = urls.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                var tasks = new List<Task>();
                foreach (var url in batchs)
                {
                    tasks.Add(ProcessRequestAsync(url, orders, cancellationToken));
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        private async Task ProcessRequestAsync(string url, OrdersModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));
            }
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            try
            {
                using var response = await _client.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync(cancellationToken) ?? throw new InvalidOperationException("reason content");

                    var order = JsonSerializer.Deserialize<Order>(body) ?? throw new InvalidOperationException("reason deserialize");

                    model.Add(order);

                    _instrumentation.AddOrder();

                    await _cacheService.SetAsync<OrdersModel>(model.Id, model, TimeSpan.FromMinutes(expiration), cancellationToken);

                    await _notificationService.BroadcastAsync(new BasicNotification()
                    {
                        Message = $" progress: {model.ProgressCounter}"
                    }, cancellationToken);

                    _logger.LogInformation("response received: " + body);
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

        [Experimental(diagnosticId: "for_parallel_request")]
        public async ValueTask<T> SendAsync<T>(HttpRequestMessage requestMessage, CancellationToken cancellationToken = default) where T : class
        {
            ArgumentNullException.ThrowIfNull(requestMessage);

            IEnumerable<string>? headerValues;

            if (requestMessage.Headers.TryGetValues("trace-id", out headerValues))
            {
                var traceId = headerValues.FirstOrDefault();

                try
                {
                    using var response = await _client.SendAsync(requestMessage, cancellationToken);

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        if (string.IsNullOrWhiteSpace(content))
                        {
                            throw new InfrastructureException($"response is null for traceId: {traceId}");
                        }

                        return JsonSerializer.Deserialize<T>(content) ?? throw new InfrastructureException("deserialize content for traceId: {traceId} ");
                    }

                }
                catch (Exception)
                {
                    _instrumentation.AddException();

                    _logger.LogError($"occurs exception when the request was sent for traceId: {traceId}");

                    throw;
                }
            }

            throw new InfrastructureException($"the request has not been sent");
        }
    }
}
