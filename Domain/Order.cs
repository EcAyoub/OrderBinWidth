namespace Domain;

public class Order
{
    public string OrderId { get; }
    public IDictionary<ProductType, int> Items { get; }
    public decimal RequiredBinWidthMm { get; }

    private Order(string orderId,
                  IDictionary<ProductType, int> items,
                  decimal requiredWidth)
    {
        OrderId = orderId;
        Items = items;
        RequiredBinWidthMm = requiredWidth;
    }

    public static Order Create(string orderId,
                               IDictionary<ProductType, int> items,
                               IBinWidthCalculator calculator)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("OrderId required.", nameof(orderId));

        if (items is null || items.Count == 0)
            throw new ArgumentException("At least one item required.", nameof(items));

        foreach (var (productType, quantity) in items)
        {
            if (quantity <= 0)
                throw new ArgumentException($"Quantity for {productType} must be > 0.");
        }

        // calcule la largeur via le service métier
        var width = calculator.Calculate(new Dictionary<ProductType, int>(items));

        return new Order(orderId, items, width);
    }
}