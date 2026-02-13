using AlGreenMES.Modules.Orders.Api.Requests;
using AlGreenMES.Modules.Orders.Application.Commands.CheckIn;
using AlGreenMES.Modules.Orders.Application.Commands.CheckOut;
using AlGreenMES.Modules.Orders.Application.Queries.GetWorkSessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Orders.Api.Controllers;

[ApiController]
[Route("api/work-sessions")]
[Authorize]
public class WorkSessionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkSessionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkSessions([FromQuery] Guid tenantId, [FromQuery] DateTime date, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetWorkSessionsQuery(tenantId, date), cancellationToken);
        return Ok(result);
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CheckInCommand(request.TenantId, request.ProcessId, request.UserId),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CheckOutCommand(request.UserId),
            cancellationToken);
        return Ok(result);
    }
}
