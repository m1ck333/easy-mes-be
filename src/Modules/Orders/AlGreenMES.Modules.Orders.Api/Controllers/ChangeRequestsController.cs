using AlGreenMES.Modules.Orders.Api.Requests;
using AlGreenMES.Modules.Orders.Application.Commands.ApproveChangeRequest;
using AlGreenMES.Modules.Orders.Application.Commands.CreateChangeRequest;
using AlGreenMES.Modules.Orders.Application.Commands.RejectChangeRequest;
using AlGreenMES.Modules.Orders.Application.Queries.GetChangeRequests;
using AlGreenMES.Modules.Orders.Application.Queries.GetMyChangeRequests;
using AlGreenMES.Modules.Orders.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Orders.Api.Controllers;

[ApiController]
[Route("api/change-requests")]
[Authorize]
public class ChangeRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChangeRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetChangeRequests([FromQuery] Guid tenantId, [FromQuery] RequestStatus? status, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetChangeRequestsQuery(tenantId, status), cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyChangeRequests([FromQuery] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMyChangeRequestsQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,SalesManager")]
    public async Task<IActionResult> CreateChangeRequest([FromBody] CreateChangeRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateChangeRequestCommand(request.TenantId, request.OrderId, request.RequestedByUserId, request.RequestType, request.Description),
            cancellationToken);
        return CreatedAtAction(nameof(GetChangeRequests), result);
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> ApproveChangeRequest(Guid id, [FromBody] HandleChangeRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new ApproveChangeRequestCommand(id, request.HandledByUserId, request.ResponseNote),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> RejectChangeRequest(Guid id, [FromBody] HandleChangeRequestRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RejectChangeRequestCommand(id, request.HandledByUserId, request.ResponseNote),
            cancellationToken);
        return Ok(result);
    }
}
