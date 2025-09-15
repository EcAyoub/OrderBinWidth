using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public sealed class EfOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public EfOrderRepository(AppDbContext db) => _db = db;

    public Task<bool> ExistsAsync(string id, CancellationToken ct = default)
        => _db.Orders.AsNoTracking().AnyAsync(o => o.OrderId == id, ct);

    public async Task SaveAsync(Order order, CancellationToken ct = default)
    {
        if (await _db.Orders.AsNoTracking().AnyAsync(o => o.OrderId == order.OrderId, ct))
            throw new InvalidOperationException($"Order '{order.OrderId}' already exists.");

        var entity = new OrderEntity
        {
            OrderId = order.OrderId,
            RequiredBinWidthMm = order.RequiredBinWidthMm,
            Items = order.Items.Select(kv => new OrderItemEntity
            {
                OrderId = order.OrderId,
                ProductType = kv.Key,   
                Quantity = kv.Value
            }).ToList()
        };

        _db.Orders.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Order?> GetAsync(string id, CancellationToken ct = default)
    {
        var entity = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == id, ct);

        if (entity is null) return null;

        var items = entity.Items.ToDictionary(i => i.ProductType, i => i.Quantity);
        return new Order(entity.OrderId, items, entity.RequiredBinWidthMm);
    }
}