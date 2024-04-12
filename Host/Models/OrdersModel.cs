using Host.Infrastructure.Metrics;
using Host.Infrastructure.Notifications;
using Host.Infrastructure.Notifications.Messages;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace Host.Models
{
    public class OrdersModel
    {
        private ConcurrentBag<Order> concurrentOrders;
        private int progress;

        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("To")]
        public string To { get; set; }

        [JsonPropertyName("From")]
        public string From { get; set; }

        [JsonPropertyName("Time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("Orders")]
        public IEnumerable<Order> Orders
        {
            get
            {
                return concurrentOrders.Select(order => order);
            }
            set
            {
                foreach (var order in value)
                {
                    concurrentOrders.Add(order);
                }

            }
        }

        [JsonPropertyName("Progress")]
        public int Progress
        {
            get => concurrentOrders.Count;            
            set => progress = value;
        }


        public async Task AddAsync(List<Order> orders, OrderInstrumentation instrumentation, INotificationService notification, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(orders);

            orders.ForEach(order => concurrentOrders.Add(order));

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
            concurrentOrders = [];
            Progress = 0;
        }
    }
}
