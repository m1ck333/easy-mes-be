using AlGreenMES.Modules.Production.Api.Requests;
using AlGreenMES.Modules.Production.Application.Commands.AddSubProcess;
using AlGreenMES.Modules.Production.Application.Commands.CreateProcess;
using AlGreenMES.Modules.Production.Application.Commands.DeactivateProcess;
using AlGreenMES.Modules.Production.Application.Commands.DeactivateSubProcess;
using AlGreenMES.Modules.Production.Application.Commands.UpdateProcess;
using AlGreenMES.Modules.Production.Application.Commands.UpdateSubProcess;
using AlGreenMES.Modules.Production.Application.Queries.GetProcessById;
using AlGreenMES.Modules.Production.Application.Queries.GetProcesses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Production.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProcessesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProcessesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProcesses([FromQuery] Guid tenantId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProcessesQuery(tenantId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProcessById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProcessByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateProcess([FromBody] CreateProcessRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateProcessCommand(request.TenantId, request.Code, request.Name, request.SequenceOrder, null),
            cancellationToken);
        return CreatedAtAction(nameof(GetProcessById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateProcess(Guid id, [FromBody] UpdateProcessRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateProcessCommand(id, request.Name, request.SequenceOrder),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateProcess(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeactivateProcessCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{processId:guid}/sub-processes")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddSubProcess(Guid processId, [FromBody] AddSubProcessRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddSubProcessCommand(processId, request.Name, request.SequenceOrder),
            cancellationToken);
        return CreatedAtAction(nameof(GetProcessById), new { id = processId }, result);
    }

    [HttpPut("{processId:guid}/sub-processes/{subProcessId:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateSubProcess(Guid processId, Guid subProcessId, [FromBody] UpdateSubProcessRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateSubProcessCommand(processId, subProcessId, request.Name, request.SequenceOrder),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{processId:guid}/sub-processes/{subProcessId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateSubProcess(Guid processId, Guid subProcessId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeactivateSubProcessCommand(processId, subProcessId), cancellationToken);
        return NoContent();
    }
}
