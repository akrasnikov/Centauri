using Host.Entity;
using Host.Events.Contracts;
using Host.Extensions;
using Host.Infrastructure.Tracing.Aspect;
using MassTransit;
using Microsoft.Extensions.Options;
using Ordering.Infrastructure.Infrastructure.Caching;
using Ordering.Infrastructure.Infrastructure.Tracing;
using System.Text.Json;
using LogContext = Serilog.Context.LogContext;


namespace Ordering.Infrastructure.Infrastructure.HttpClients
{

    public class IntegrationClient
    {
        private readonly HttpClient _client;
        private readonly ICacheService _cacheService;
        readonly IBus _bus;
        private readonly List<RequestInfo> _requests;
        private readonly ILogger<IntegrationClient> _logger;

        public IntegrationClient(
            HttpClient client,
            ILogger<IntegrationClient> logger,
            ICacheService cacheService,
            IOptions<IntegrationOptions> options,
            IBus bus)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _requests = options.Value.Requests ?? throw new ArgumentNullException(nameof(options));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
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
                    var orders =
                        await response
                        .Content
                        .ReadFromJsonAsync<List<Order>>(cancellationToken)
                        ?? throw new InvalidOperationException("reason content");

                    model.Add(orders);

                    await _bus.Publish(new OrderCreated()
                    {
                        Id = model.Id,
                        Progress = model.Progress

                    }, cancellationToken);

                    _logger.LogInformation($"response received -> id: {model.Id} - list : {JsonSerializer.Serialize(orders)}");

                    return;
                }

                _logger.LogError($"orders id: {model.Id} failed with status code {response.StatusCode}");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"orders id: {model.Id} for url: {url}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"orders id: {model.Id} for url: {url}: {ex.Message}");
            }

            await _bus.Publish(new OrderCanceled()
            {

            }, cancellationToken);
        }
    }
}
