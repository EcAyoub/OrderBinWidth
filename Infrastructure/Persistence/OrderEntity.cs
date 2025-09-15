using Domain;

namespace Infrastructure.Persistence;

public class OrderEntity
{
    public string OrderId { get; set; } = default!;
    public decimal RequiredBinWidthMm { get; set; }

    public List<OrderItemEntity> Items { get; set; } = new();
}

public class OrderItemEntity
{
    public string OrderId { get; set; } = default!;
    public ProductType ProductType { get; set; }
    public int Quantity { get; set; }

    public OrderEntity Order { get; set; } = default!;
}
