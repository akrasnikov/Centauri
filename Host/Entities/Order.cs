namespace Host.Entities
{
    public class Order
    {
        public required Guid Id { get; set; }
        public required string DepartureCity { get; set; }
        public required string ArrivalCity { get; set; }
        public required DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public double Price { get; set; }
        public required string AirLine { get; set; }
    }
}
