using System.Collections.Concurrent;

namespace Ordering.Domain.Entity
{
    public class Orders(string id, string from, string to, DateTime time)
    {

        public string Id { get; set; } = id ?? throw new ArgumentNullException(nameof(id));

        public string To { get; set; } = to ?? throw new ArgumentNullException(nameof(to));

        public string From { get; set; } = from ?? throw new ArgumentNullException(nameof(from));

        public DateTime Time { get; set; } = time;

        public ConcurrentBag<Order> Items { get; set; } = [];


        public int Progress
        {
            get => Items.Count;
            //set => progress = value;
        }


         
        public void Add(List<Order> orders)
        {
            ArgumentNullException.ThrowIfNull(orders);

            foreach (var order in orders)
            {

                Items.Add(order);
            }

        }
    }
}
