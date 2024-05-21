﻿using MassTransit;
using Ordering.Domain.DomainEvents.Contracts;
using Ordering.Domain.Tracing.Aspect;
using Ordering.Infrastructure.Metrics;
using Ordering.Infrastructure.Notifications;
using Ordering.Infrastructure.Tracing.Aspect;

namespace Ordering.Worker.Events.Consumers
{
    public class OrderCanceledConsumer : IConsumer<OrderCanceled>
    {
        private readonly OrderInstrumentation _instrumentation;
        private readonly ILogger<OrderCanceledConsumer> _logger;
        private readonly INotificationService _notification;

        public OrderCanceledConsumer(
            OrderInstrumentation instrumentation,
            ILogger<OrderCanceledConsumer> logger,
            INotificationService notification)
        {
            _instrumentation = instrumentation ?? throw new ArgumentNullException(nameof(instrumentation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
        }

        [TracingInterceptor(ActivityName = "canceled order")]
        public async Task Consume(ConsumeContext<OrderCanceled> context)
        {
            _instrumentation.AddOrder(-1);

            await Task.CompletedTask;

            //await _notification.BroadcastAsync(new BasicNotification()
            //{
            //    Message = $" progress: {1}"
            //}, context.CancellationToken);

        }
    }
}