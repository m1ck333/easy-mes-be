using AlGreenMES.Modules.Orders.Api.Requests;
using AlGreenMES.Modules.Orders.Application.Commands.BlockProcess;
using AlGreenMES.Modules.Orders.Application.Commands.CompleteProcess;
using AlGreenMES.Modules.Orders.Application.Commands.PauseStation;
using AlGreenMES.Modules.Orders.Application.Commands.ResumeProcessWork;
using AlGreenMES.Modules.Orders.Application.Commands.ResumeStation;
using AlGreenMES.Modules.Orders.Application.Commands.StartProcessWork;
using AlGreenMES.Modules.Orders.Application.Commands.StopProcessWork;
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

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> StartWork(Guid id, [FromBody] StartProcessWorkRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartProcessWorkCommand(id, request.UserId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/stop")]
    public async Task<IActionResult> StopWork(Guid id, [FromBody] StopProcessWorkRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new StopProcessWorkCommand(id, request.UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/resume")]
    public async Task<IActionResult> ResumeWork(Guid id, [FromBody] ResumeProcessWorkRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ResumeProcessWorkCommand(id, request.UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("pause-station")]
    public async Task<IActionResult> PauseStation([FromBody] StationRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new PauseStationCommand(request.ProcessId, request.TenantId, request.UserId), cancellationToken);
        return NoContent();
    }

    [HttpPost("resume-station")]
    public async Task<IActionResult> ResumeStation([FromBody] StationRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ResumeStationCommand(request.ProcessId, request.TenantId, request.UserId), cancellationToken);
        return NoContent();
    }
}
