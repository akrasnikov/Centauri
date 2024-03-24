using Hangfire;
using Host.Entities;
using Host.Interfaces;
using Host.Requests;

namespace Host.Services
{
    public class OrderAggregator : IOrderAggregator
    {

        private readonly IBackgroundJobClient _background;
        private readonly ILogger<OrderAggregator> _logger;

        public OrderAggregator(IBackgroundJobClient background, ILogger<OrderAggregator> logger)
        {
            _background = background ?? throw new ArgumentNullException(nameof(background));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        [Queue("default")]
        private void EmptyDefault()
        {
        }
    }
}
