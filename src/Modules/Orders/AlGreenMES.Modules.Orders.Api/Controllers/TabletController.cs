using AlGreenMES.Modules.Orders.Application.Commands.PauseWork;
using AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletActiveWork;
using AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletIncoming;
using AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletQueue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Orders.Api.Controllers;

[ApiController]
[Route("api/tablet")]
[Authorize]
public class TabletController : ControllerBase
{
    private readonly IMediator _mediator;

    public TabletController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("queue")]
    public async Task<IActionResult> GetQueue([FromQuery] Guid tenantId, [FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTabletQueueQuery(tenantId, userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveWork([FromQuery] Guid tenantId, [FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTabletActiveWorkQuery(tenantId, userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncoming([FromQuery] Guid tenantId, [FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTabletIncomingQuery(tenantId, userId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("pause")]
    public async Task<IActionResult> Pause([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new PauseWorkCommand(userId), cancellationToken);
        return Ok();
    }
}
