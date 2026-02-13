using AlGreenMES.Modules.Orders.Api.Requests;
using AlGreenMES.Modules.Orders.Application.Commands.BlockProcess;
using AlGreenMES.Modules.Orders.Application.Commands.CompleteProcess;
using AlGreenMES.Modules.Orders.Application.Commands.UnblockProcess;
using AlGreenMES.Modules.Orders.Application.Commands.WithdrawProcess;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Orders.Api.Controllers;

[ApiController]
[Route("api/order-item-processes")]
[Authorize]
public class ProcessWorkflowController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProcessWorkflowController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{id:guid}/block")]
    public async Task<IActionResult> BlockProcess(Guid id, [FromBody] BlockProcessRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new BlockProcessCommand(id, request.UserId, request.Reason), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/unblock")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> UnblockProcess(Guid id, [FromBody] UnblockProcessRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UnblockProcessCommand(id, request.UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteProcess(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CompleteProcessCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/withdraw")]
    [Authorize(Roles = "Admin,Manager,Coordinator")]
    public async Task<IActionResult> WithdrawProcess(Guid id, [FromBody] WithdrawProcessRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new WithdrawProcessCommand(id, request.UserId, request.Reason), cancellationToken);
        return NoContent();
    }
}
