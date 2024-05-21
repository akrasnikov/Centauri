namespace Ordering.Domain.DomainEvents.Contracts
{
    public class OrderCreatedEvent
    {
        public string Id { get; set; }
        public int Progress { get; set; }
    }
}
