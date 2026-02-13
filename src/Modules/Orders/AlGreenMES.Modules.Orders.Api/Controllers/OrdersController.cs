using AlGreenMES.Modules.Orders.Api.Requests;
using AlGreenMES.Modules.Orders.Application.Commands.ActivateOrder;
using AlGreenMES.Modules.Orders.Application.Commands.AddOrderItem;
using AlGreenMES.Modules.Orders.Application.Commands.CancelOrder;
using AlGreenMES.Modules.Orders.Application.Commands.CreateOrder;
using AlGreenMES.Modules.Orders.Application.Commands.PauseOrder;
using AlGreenMES.Modules.Orders.Application.Commands.RemoveOrderItem;
using AlGreenMES.Modules.Orders.Application.Commands.ResumeOrder;
using AlGreenMES.Modules.Orders.Application.Commands.UpdateOrder;
using AlGreenMES.Modules.Orders.Application.Queries.GetOrderById;
using AlGreenMES.Modules.Orders.Application.Queries.GetOrders;
using AlGreenMES.Modules.Orders.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Orders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] Guid tenantId, [FromQuery] OrderStatus? status, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrdersQuery(tenantId, status), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,SalesManager")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateOrderCommand(request.TenantId, request.OrderNumber, request.DeliveryDate, request.Priority, request.OrderType, Guid.Empty, request.Notes),
            cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager,SalesManager")]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateOrderCommand(id, request.Notes, request.CustomWarningDays, request.CustomCriticalDays),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> ActivateOrder(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ActivateOrderCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/pause")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> PauseOrder(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new PauseOrderCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/resume")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> ResumeOrder(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ResumeOrderCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{orderId:guid}/items")]
    [Authorize(Roles = "Admin,Manager,SalesManager")]
    public async Task<IActionResult> AddOrderItem(Guid orderId, [FromBody] AddOrderItemRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddOrderItemCommand(orderId, request.ProductCategoryId, request.ProductName, request.Quantity, request.Notes),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    [Authorize(Roles = "Admin,Manager,SalesManager")]
    public async Task<IActionResult> RemoveOrderItem(Guid orderId, Guid itemId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveOrderItemCommand(orderId, itemId), cancellationToken);
        return NoContent();
    }
}
