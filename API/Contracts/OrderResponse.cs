using Domain;

namespace API.Contracts;

public class OrderResponse
{
    public string OrderId { get; set; } = default!;
    public Dictionary<ProductType, int> Items { get; set; } = new();
    public decimal RequiredBinWidth { get; set; }
}
