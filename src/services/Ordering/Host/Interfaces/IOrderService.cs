using Ordering.Host.Common.Interfaces;
using Ordering.Host.Models;
using Ordering.Host.Requests;

namespace Ordering.Host.Interfaces
{
    public interface IOrderService : ITransientService
    {
        Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default);
        OrderModel Create(OrderRequest request, CancellationToken cancellationToken = default);
    }
}
