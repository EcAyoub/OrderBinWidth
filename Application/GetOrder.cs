using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application;

public class GetOrder
{
    private readonly IOrderRepository _repo;
    public GetOrder(IOrderRepository repo) => _repo = repo;

    public record Result(string OrderId, Dictionary<ProductType, int> Items, decimal RequiredBinWidth);

    public async Task<Result?> Handle(string orderId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("OrderId required.", nameof(orderId));

        var order = await _repo.GetAsync(orderId, ct);
        if (order is null) return null;

        var itemsCopy = order.Items is Dictionary<ProductType, int> d ? new(d) : new Dictionary<ProductType, int>(order.Items);
        return new Result(order.OrderId, itemsCopy, order.RequiredBinWidthMm);
    }
}
