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
    public async Task<IActionResult> Create([FromBody] CreateOrderDto body, CancellationToken ct)
    {
        if (body is null || string.IsNullOrWhiteSpace(body.OrderId) || body.Items is null || body.Items.Count == 0)
            return ValidationProblem("OrderId and at least one item are required.");

        try
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
        catch (ArgumentException ex)
        {
            return ValidationProblem(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new ProblemDetails { Title = "Order already exists", Detail = ex.Message });
        }
    }

    /// <summary>Get an order by id.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] string id, CancellationToken ct)
    {
        var result = await _getOrder.Handle(id, ct);
        if (result is null) return NotFound();

        return Ok(new OrderResponse
        {
            OrderId = result.OrderId,
            Items = result.Items,
            RequiredBinWidth = result.RequiredBinWidth
        });
    }
}
