using Host.Infrastructure.Tracing.Aspect;
using System.Collections.Concurrent;

namespace Host.Entity
{
    public class Orders(string id, string from, string to, DateTime time)
    {
        private ConcurrentBag<Order> concurrentOrders = [];

        //private int progress;

        public string Id { get; set; } = id ?? throw new ArgumentNullException(nameof(id));

        public string To { get; set; } = to ?? throw new ArgumentNullException(nameof(to));

        public string From { get; set; } = from ?? throw new ArgumentNullException(nameof(from));

        public DateTime Time { get; set; } = time;

        public IEnumerable<Order> Items
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

        public int Progress
        {
            get => concurrentOrders.Count;
            //set => progress = value;
        }


        [OrderTracingInterceptor]
        public void Add(List<Order> orders)
        {
            ArgumentNullException.ThrowIfNull(orders);

            foreach (var order in orders)
            {

                concurrentOrders.Add(order);
            }

        }
    }
}
