using Host.Infrastructure.Notifications;
using Host.Infrastructure.Notifications.Messages;
using Host.Metrics;

namespace Host.Models
{
    public class OrdersModel
    {
        public string Id { get; }
        public string To { get; set; }
        public string From { get; set; }
        public DateTime Time { get; set; }
        public List<OrderModel> Items { get; }
        public int Progress { get; set; }

        public async Task AddAsync(OrderModel order, OrderInstrumentation instrumentation, INotificationService notification, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(order);           
            Items.Add(order);
            Progress++;

            instrumentation.AddOrder();
            await notification.BroadcastAsync(new BasicNotification()
            {
                Message = $" progress: {Progress}"
            }, cancellationToken);
        }

        public OrdersModel(string id, string from, string to, DateTime time)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            To = to ?? throw new ArgumentNullException(nameof(to));
            From = from ?? throw new ArgumentNullException(nameof(from));
            Time = time;
            Items = [];
            Progress = 0;
        }
    }
}
