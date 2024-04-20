using Host.Common.Interfaces;
using Host.Models;
using Host.Requests;

namespace Host.Interfaces
{
    public interface IOrderService : ITransientService
    {
        Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default);
        OrderModel Create(OrderRequest request,  CancellationToken cancellationToken = default);
    }
}
