using Domain;
using Infrastructure;

namespace Application;

public class CreateOrder
{
    private readonly IOrderRepository _repo;
    private readonly IBinWidthCalculator _calc;

    public CreateOrder(IOrderRepository repo, IBinWidthCalculator calc)
    { _repo = repo; _calc = calc; }

    public record Item(ProductType ProductType, int Quantity);
    public record Command(string OrderId, IReadOnlyList<Item> Items);
    public record Result(string OrderId, Dictionary<ProductType, int> Items, decimal RequiredBinWidth);

    public async Task<Result> Handle(Command cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.OrderId))
            throw new ArgumentException("OrderId required.", nameof(cmd.OrderId));
        if (cmd.Items is null || cmd.Items.Count == 0)
            throw new ArgumentException("At least one item required.", nameof(cmd.Items));

        if (await _repo.ExistsAsync(cmd.OrderId, ct))
            throw new InvalidOperationException($"Order '{cmd.OrderId}' already exists.");

        // Agrégation par type (+ validation qty > 0)
        var dict = new Dictionary<ProductType, int>();
        foreach (var it in cmd.Items)
        {
            if (it.Quantity <= 0)
                throw new ArgumentException($"Quantity for {it.ProductType} must be > 0.");
            dict[it.ProductType] = dict.TryGetValue(it.ProductType, out var q) ? q + it.Quantity : it.Quantity;
        }

        var order = Order.Create(cmd.OrderId, dict, _calc);
        await _repo.SaveAsync(order, ct);

        return new Result(order.OrderId, new Dictionary<ProductType, int>(dict), order.RequiredBinWidthMm);

    }
}
