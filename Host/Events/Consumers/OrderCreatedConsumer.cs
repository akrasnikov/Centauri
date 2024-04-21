using Host.Events.Contracts;
using Host.Infrastructure.Metrics;
using Host.Infrastructure.Notifications;
using Host.Infrastructure.Notifications.Messages;
using Host.Infrastructure.Tracing.Aspect;
using MassTransit;

namespace Host.Events.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly OrderInstrumentation _instrumentation;
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly INotificationService _notification ;

        public OrderCreatedConsumer(
            OrderInstrumentation instrumentation, 
            ILogger<OrderCreatedConsumer> logger,
            INotificationService notification)
        {
            _instrumentation = instrumentation ?? throw new ArgumentNullException(nameof(instrumentation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }

        [OrderTracingInterceptor(ActivityName = "order created")]
        public async Task Consume(ConsumeContext<OrderCreated> context )
        {
            _instrumentation.AddOrder(1);

            await _notification.BroadcastAsync(new BasicNotification()
            {
                Label = BasicNotification.LabelType.Success,
                Id = context.Message.Id,
                Progress = context.Message.Progress
            }, context.CancellationToken);
            
        }
    }
}
