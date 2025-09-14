using Domain;

namespace Infrastructure;

public interface IOrderRepository
{
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);
    Task SaveAsync(Order order, CancellationToken ct = default);
    Task<Order?> GetAsync(string id, CancellationToken ct = default);
}
