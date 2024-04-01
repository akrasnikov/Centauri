using Hangfire;
using Host.Caching;
using Host.Entities;
using Host.Infrastructure.Integrations;
using Host.Interfaces;
using Host.Models;
using Host.Options;
using Host.Requests;
using Microsoft.Extensions.Options;

namespace Host.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBackgroundJobClient _job;
        private readonly IntegrationClient _integration;
        private readonly ICacheService _cacheService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IBackgroundJobClient job,
            ILogger<OrderService> logger,
            IntegrationClient orderIntegration,
            IOptions<IntegrationOptions> options,
            ICacheService cacheService)
        {
            _job = job ?? throw new ArgumentNullException(nameof(job));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _integration = orderIntegration ?? throw new ArgumentNullException(nameof(orderIntegration));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<Order> CreateAsync(SearchRequest request, CancellationToken cancellationToken = default)
        {
            var order = new Order()
            {
                Id = Guid.NewGuid(),
                From = request.From,
                To = request.To,
                Time = request.Time
            };
            _job.Enqueue(() => _integration.SendAsync(order, cancellationToken));
            return await Task.FromResult(order);
        }

        public async Task<AggregatedDataModel> GetAsync(string id, CancellationToken cancellationToken = default)
        {
          return await _cacheService.GetAsync<AggregatedDataModel>(id, cancellationToken) ?? throw new BadHttpRequestException("no key exists");           
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
