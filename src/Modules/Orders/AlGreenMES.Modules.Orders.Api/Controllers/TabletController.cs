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
    public async Task<IActionResult> GetQueue([FromQuery] Guid processId, [FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTabletQueueQuery(processId, tenantId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveWork([FromQuery] Guid processId, [FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTabletActiveWorkQuery(processId, tenantId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncoming([FromQuery] Guid processId, [FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTabletIncomingQuery(processId, tenantId), cancellationToken);
        return Ok(result);
    }
}
