using Host.Entities;
using Host.Models;
using Host.Requests;

namespace Host.Interfaces
{
    public interface IOrderService
    {
        Task<AggregatedDataModel> GetAsync(string id, CancellationToken cancellationToken = default)
        Task<Order> CreateAsync(SearchRequest request, CancellationToken cancellationToken = default);
    }
}
