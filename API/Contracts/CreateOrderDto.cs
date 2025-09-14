using Domain;

namespace API.Contracts;

/// <summary>
/// Crée une commande et calcule la largeur du bac.
/// </summary>
public class CreateOrderDto
{
    public string OrderId { get; set; } = default!;
    public List<ItemDto> Items { get; set; } = new();
}

public  class ItemDto
{
    public ProductType ProductType { get; set; }
    public int Quantity { get; set; }
}