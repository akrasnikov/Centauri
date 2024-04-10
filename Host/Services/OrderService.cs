using Hangfire;
using Host.Common.Interfaces;
using Host.Infrastructure.Caching;
using Host.Infrastructure.HttpClients;
using Host.Infrastructure.Logging.PostSharp;
using Host.Infrastructure.Tracing;
using Host.Interfaces;
using Host.Models;
using Host.Requests;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using static System.Net.Mime.MediaTypeNames;

namespace Host.Services
{
    [CustomLoggingFormatter]
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

        public async Task<OrdersModel> CreateAsync(OrderRequest request, CancellationToken cancellationToken = default)
        {
            var order = new OrdersModel($"{Guid.NewGuid()}", request.From, request.To, request.Time);           
           
            _job.Enqueue(() => _integration.SendAsync(order, cancellationToken));

            return order;
        }

        public async Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("start get order by id");
            using var activitySource = ActivityProvider.Create();
            using var activity = activitySource.StartActivity($"{ActivityProvider.MethodName}: {nameof(GetAsync).ToLowerInvariant()}");
            activity?.SetTag(ActivityProvider.MethodArgument, $"id: {id}");
            return await _cacheService.GetAsync<OrdersModel>(id, cancellationToken) ?? throw new BadHttpRequestException("no key exists");           
        } 
         
    }
}
