using Hangfire;
using Microsoft.Extensions.Options;
using Ordering.Host.Requests;

namespace Ordering.Host.Services
{
    //[CustomLoggingFormatter]
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

        [TracingInterceptor(ActivityName = "create bakground task")]
        public OrderModel Create(OrderRequest request, CancellationToken cancellationToken = default)
        {
            var order = new Orders($"{Guid.NewGuid()}", request.From, request.To, request.Time);

            _job.Enqueue(() => _integration.SendAsync(order, cancellationToken));

            return new OrderModel()
            {
                Id = order.Id,
            };
        }

        [TracingInterceptor(ActivityName = "get orders")]
        public async Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _cacheService.GetAsync<OrdersModel>(id, cancellationToken) ?? throw new BadHttpRequestException("no key exists");
        }

    }
}
