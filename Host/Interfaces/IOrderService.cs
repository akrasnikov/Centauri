using Host.Entities;
using Host.Requests;

namespace Host.Interfaces
{
    public interface IOrderService
    {
        Task<IReadOnlyCollection<Order>> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Order> CreateAsync(SearchRequest request, CancellationToken cancellationToken = default);
    }
}
