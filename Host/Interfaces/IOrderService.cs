using Host.Models;
using Host.Requests;

namespace Host.Interfaces
{
    public interface IOrderService
    {
        Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default);
        Task<OrdersModel> CreateAsync(OrderRequest request, CancellationToken cancellationToken = default);
    }
}
