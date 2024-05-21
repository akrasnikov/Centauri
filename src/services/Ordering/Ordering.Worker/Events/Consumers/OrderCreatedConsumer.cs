using MassTransit;
using Ordering.Domain.DomainEvents.Contracts;
using Ordering.Infrastructure.Metrics;
using Ordering.Infrastructure.Notifications;
using Ordering.Infrastructure.Notifications.Messages;
using Ordering.Infrastructure.Tracing.Aspect;

namespace Ordering.Worker.Events.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly OrderInstrumentation _instrumentation;
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly INotificationService _notification;

        public OrderCreatedConsumer(
            OrderInstrumentation instrumentation,
            ILogger<OrderCreatedConsumer> logger,
            INotificationService notification)
        {
            _instrumentation = instrumentation ?? throw new ArgumentNullException(nameof(instrumentation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }

        [TracingInterceptor(ActivityName = "order created")]
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
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
