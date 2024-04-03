namespace Host.Models
{
#nullable disable
    public class OrderModel
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
