using API.Contracts;
using Application;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrder _createOrder;
    private readonly GetOrder _getOrder;

    public OrdersController(CreateOrder createOrder, GetOrder getOrder)
    {
        _createOrder = createOrder;
        _getOrder = getOrder;
    }

    /// <summary>Create a new order and returns required bin width.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderDto body, CancellationToken ct)
    {
        var command = new CreateOrder.Command(
            body.OrderId,
            body.Items.Select(i => new CreateOrder.Item(i.ProductType, i.Quantity)).ToList()
        );

        var result = await _createOrder.Handle(command, ct);

        var response = new OrderResponse
        {
            OrderId = result.OrderId,
            Items = result.Items,
            RequiredBinWidth = result.RequiredBinWidth
        };

        return CreatedAtAction(nameof(GetById), new { id = response.OrderId }, response);
    }

    /// <summary>Get an order by id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetById([FromRoute] string id, CancellationToken ct)
    {
        var result = await _getOrder.Handle(id, ct);
        if (result is null) return NotFound();

        return new OrderResponse
        {
            OrderId = result.OrderId,
            Items = result.Items,
            RequiredBinWidth = result.RequiredBinWidth
        };
    }
}
