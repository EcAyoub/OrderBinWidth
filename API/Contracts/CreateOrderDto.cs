using System.ComponentModel.DataAnnotations;
using Domain;

namespace API.Contracts;

public class CreateOrderDto
{
    [Required, MinLength(1)]
    public string OrderId { get; set; } = default!;

    [Required, MinLength(1, ErrorMessage = "At least one article is requested.")]
    public List<ItemDto> Items { get; set; } = new();
}

public class ItemDto
{
    [Required]
    [EnumDataType(typeof(ProductType))]
    public ProductType ProductType { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be >= 1")]
    public int Quantity { get; set; }
}