using Hangfire;
using Host.Entities;
using Host.Infrastructure.Integrations;
using Host.Interfaces;
using Host.Requests;

namespace Host.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBackgroundJobClient _background;
        private readonly IntegrationClient _orderIntegration;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IBackgroundJobClient background, ILogger<OrderService> logger, IntegrationClient orderIntegration)
        {
            _background = background ?? throw new ArgumentNullException(nameof(background));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderIntegration = orderIntegration ?? throw new ArgumentNullException(nameof(orderIntegration));
        }

        public Task<Order> CreateAsync(SearchRequest request, CancellationToken cancellationToken = default)
        {
            _background.Enqueue(() => EmptyDefault());

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
                //tasks.Add(_orderIntegration.GetOrder(currentIds));
            }

            return (await Task.WhenAll(tasks)).SelectMany(u => u);
        }

        [Queue("default")]
        private void EmptyDefault()
        {
        }
    }
}
