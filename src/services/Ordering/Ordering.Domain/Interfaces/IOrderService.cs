using Ordering.Domain.Common.Interfaces;
using Ordering.Domain.Models;
using Ordering.Host.Requests;

namespace Ordering.Domain.Interfaces
{
    public interface IOrderService : ITransientService
    {
        Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default);
        OrderModel Create(OrderRequest request, CancellationToken cancellationToken = default);
    }
}
