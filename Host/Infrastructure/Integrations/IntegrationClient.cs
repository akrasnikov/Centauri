using Host.Caching;
using Host.Entities;
using Host.Exceptions;
using Host.Infrastructure.Notifications;
using Host.Metrics;
using Host.Models;
using Host.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Host.Infrastructure.Integrations
{
    public class IntegrationClient
    {
        private readonly HttpClient _client;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly MetricsInstrumentation _instrumentation;
        private readonly IntegrationOptions _options;
        private readonly List<RequestInfo> _requests;
        private readonly ILogger<IntegrationClient> _logger;

        private const string prefix = "orders:";
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

            _client.Timeout = new TimeSpan(0, 10, 0);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _notificationService = notification ?? throw new ArgumentNullException(nameof(notification));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _requests = options.Value.Requests ?? throw new ArgumentNullException(nameof(options));
        }



        public List<string> RequestFromOptions(Order order)
        {
            var results = new List<string>();

            foreach (var request in _requests)
            {
                var url = $"{request.Url}?from={order.From}&to={order.To}&time={order.Time}";
                results.Add(url);
            }

            return results;
        }

        public async Task<string> SendAsync(Order order, CancellationToken cancellationToken = default)
        {
            int batchSize = 10;

            //var urls = new string[]
            //{
            //    $"https://api.example.com/endpoint1&{order.From}&{order.To}&{order.Time}",
            //    $"https://api.example.com/endpoint2&{order.From}&{order.To}&{order.Time}"
            //};

            var urls = RequestFromOptions(order);

            int totalBatches = (int)Math.Ceiling((double)urls.Count / batchSize);

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var batchUrls = urls.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                var tasks = new List<Task>();

                foreach (var url in batchUrls)
                {
                    tasks.Add(ProcessRequestAsync(url, cancellationToken));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            return $"Orders sent for {order.From} to {order.To} at {order.Time}";
        }

        private async Task ProcessRequestAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                using var response = await _client.GetAsync(url, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync(cancellationToken) ?? throw new InvalidOperationException("reason content");

                    var order = JsonSerializer.Deserialize<Order>(body) ?? throw new InvalidOperationException("reason deserialize");

                    string chacheKey = $"{prefix}:{url}";

                    var model = await _cacheService.GetAsync<OrdersModel>(chacheKey, cancellationToken) ?? new OrdersModel();

                    model.Items.Add(order);

                    model.ProgressCounter++;

                    await _cacheService.SetAsync<OrdersModel>(chacheKey, model, TimeSpan.FromMinutes(expiration), cancellationToken);

                    await _notificationService.BroadcastAsync(new BasicNotification()
                    {
                        Message = $" progress: {model.ProgressCounter}"
                    },
                    cancellationToken);

                    _logger.LogInformation("response received: " + body);

                    _instrumentation.AddOrder();
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
