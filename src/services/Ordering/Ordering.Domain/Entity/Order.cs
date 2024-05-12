namespace Ordering.Domain.Entity
{
#nullable disable
    public class Order
    {
        public string Id { get; set; }

        public DateTime? Time { get; set; }

        public string From { get; set; }

        public string To { get; set; }
    }
}
