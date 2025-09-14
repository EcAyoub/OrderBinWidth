using Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<string, Order> _db = new();

    public Task<bool> ExistsAsync(string id, CancellationToken ct = default)
        => Task.FromResult(_db.ContainsKey(id));

    public Task SaveAsync(Order order, CancellationToken ct = default)
    { _db[order.OrderId] = order; return Task.CompletedTask; }

    public Task<Order?> GetAsync(string id, CancellationToken ct = default)
    { _db.TryGetValue(id, out var o); return Task.FromResult(o); }
}
