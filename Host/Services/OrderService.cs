using Hangfire;
using Host.Entities;
using Host.Infrastructure;
using Host.Infrastructure.Integrations;
using Host.Interfaces;
using Host.Options;
using Host.Requests;
using Microsoft.Extensions.Options;

namespace Host.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBackgroundJobClient _background;
        private readonly IntegrationClient _integration;
        private readonly IOptions<IntegrationOptions> _options;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IBackgroundJobClient background,
            ILogger<OrderService> logger,
            IntegrationClient orderIntegration,
            IOptions<IntegrationOptions> options)
        {
            _background = background ?? throw new ArgumentNullException(nameof(background));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _integration = orderIntegration ?? throw new ArgumentNullException(nameof(orderIntegration));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Order> CreateAsync(SearchRequest request, CancellationToken cancellationToken = default)
        {
            using var requestMessage = RequestMessageFactory.Create(HttpMethod.Get, null, "", _options);

            var response = await _integration.SendAsync<Order>(requestMessage, cancellationToken);
             

            return default;
        }

        public Task<IReadOnlyCollection<Order>> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<Order>> GetOrdersWithBatches(IEnumerable<int> userIds)
        {
            var tasks = new List<Task<IEnumerable<Order>>>();
            var batchSize = 100;
            int numberOfBatches = (int)Math.Ceiling((double)userIds.Count() / batchSize);

            for (int i = 0; i < numberOfBatches; i++)
            {
                var currentIds = userIds.Skip(i * batchSize).Take(batchSize);
                //tasks.Add(_ntegration.GetOrder(currentIds));
            }

            return (await Task.WhenAll(tasks)).SelectMany(u => u);
        }

        [Queue("default")]
        private void EmptyDefault()
        {
        }
    }
}
