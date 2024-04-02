using Host.Entities;

namespace Host.Models
{
    public class OrdersModel
    {
        public string Id { get; }
        public string To { get; set; }
        public string From { get; set; }
        public DateTime Time { get; set; }
        public List<Order> Items { get; }
        public int ProgressCounter { get; set; }

        public void Add(Order order)
        {
            ArgumentNullException.ThrowIfNull(order);
            Items.Add(order);
            ProgressCounter++;
        }

        public OrdersModel(string id, string from, string to, DateTime time)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            To = to ?? throw new ArgumentNullException(nameof(to));
            From = from ?? throw new ArgumentNullException(nameof(from));
            Time = time;
            Items = [];
            ProgressCounter = 0;
        }
    }
}
