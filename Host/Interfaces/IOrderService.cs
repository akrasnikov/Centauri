using Host.Common.Interfaces;
using Host.Models;
using Host.Requests;

namespace Host.Interfaces
{
    public interface IOrderService : ITransientService
    {
        Task<OrdersModel> GetAsync(string id, CancellationToken cancellationToken = default);
        Task<OrdersModel> CreateAsync(OrderRequest request, string correlationId, CancellationToken cancellationToken = default);
    }
}
