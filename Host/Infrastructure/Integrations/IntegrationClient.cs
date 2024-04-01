using Host.Caching;
using Host.Entities;
using Host.Exceptions;
using Host.Infrastructure.Notifications;
using Host.Metrics;
using Host.Models;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Host.Infrastructure.Integrations
{
    public class IntegrationClient
    {
        private readonly HttpClient _client;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly MetricsInstrumentation _instrumentation;
        private readonly ILogger<IntegrationClient> _logger;

        private const string prefix = "orders:";
        private const double expiration = 1 /*minute*/;

        public IntegrationClient(HttpClient client, MetricsInstrumentation instrumentation, ILogger<IntegrationClient> logger, ICacheService cacheService, INotificationService notificationService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _instrumentation = instrumentation ?? throw new ArgumentNullException(nameof(instrumentation));

            _client.Timeout = new TimeSpan(0, 10, 0);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<string> SendAsync(Order order, CancellationToken cancellationToken = default)
        {
            int batchSize = 10;
            // List of URLs to send requests to
            var urls = new string[]
            {
                $"https://api.example.com/endpoint1&{order.From}&{order.To}&{order.Time} ",
                $"https://api.example.com/endpoint2&{order.From}&{order.To}&{order.Time}",
            };

            int totalBatches = (int)Math.Ceiling((double)urls.Length / batchSize);
            byte[] bytesKey = Encoding.UTF8.GetBytes($"{order.From}&{order.To}&{order.Time}");

            var hashKey = SHA256.HashData(bytesKey);

            string chacheKey = $"{prefix}:{hashKey}";

            for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
            {
                var batchUrls = urls.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                var requestTasks = new List<Task<HttpResponseMessage>>();

                foreach (var url in batchUrls) requestTasks.Add(_client.GetAsync(url, cancellationToken));

                var responses = await Task.WhenAll(requestTasks).ConfigureAwait(false);

                foreach (var response in responses)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync(cancellationToken);

                        var orderDeserialize = JsonSerializer.Deserialize<Order>(body);

                        if (orderDeserialize is null) continue;

                        var model = await _cacheService.GetAsync<AggregatedDataModel>(chacheKey, cancellationToken) ?? new AggregatedDataModel();

                        model.Items.Add(order);

                        model.ProgressCounter++;

                        await _cacheService.SetAsync<AggregatedDataModel>(chacheKey, model, TimeSpan.FromMinutes(expiration), cancellationToken);

                        var message = new BasicNotification()
                        {
                            Message = $" progress: {model.ProgressCounter}"
                        };

                        await _notificationService.BroadcastAsync(message, cancellationToken);

                        _logger.LogInformation("response received: " + body);

                        _instrumentation.AddOrder();
                    }
                    _logger.LogInformation("request failed with status code: " + response.StatusCode);
                }
            }
            return chacheKey;
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
