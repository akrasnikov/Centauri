using Host.Infrastructure.Metrics;
using Host.Infrastructure.Notifications;
using Host.Infrastructure.Notifications.Messages;
using System.Text.Json.Serialization;

namespace Host.Models
{
    public class OrdersModel
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("To")]
        public string To { get; set; }

        [JsonPropertyName("From")]
        public string From { get; set; }

        [JsonPropertyName("Time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("Orders")]
        public List<Order> Orders { get; set; }

        [JsonPropertyName("Progress")]
        public int Progress { get; set; }

        public async Task AddAsync(IReadOnlyList<Order> orders, OrderInstrumentation instrumentation, INotificationService notification, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(orders);           
            Orders.AddRange(orders);
            Progress += orders.Count;

            instrumentation.AddOrder(Progress);

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
            Orders = [];
            Progress = 0;
        }
    }
}
