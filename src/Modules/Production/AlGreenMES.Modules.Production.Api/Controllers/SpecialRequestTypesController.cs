using AlGreenMES.Modules.Production.Api.Requests;
using AlGreenMES.Modules.Production.Application.Commands.CreateSpecialRequestType;
using AlGreenMES.Modules.Production.Application.Commands.DeactivateSpecialRequestType;
using AlGreenMES.Modules.Production.Application.Commands.UpdateSpecialRequestType;
using AlGreenMES.Modules.Production.Application.Queries.GetSpecialRequestTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Production.Api.Controllers;

[ApiController]
[Route("api/special-request-types")]
[Authorize]
public class SpecialRequestTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpecialRequestTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSpecialRequestTypes(
        [FromQuery] Guid tenantId,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetSpecialRequestTypesQuery
        {
            TenantId = tenantId,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize,
            Search = search
        }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateSpecialRequestType([FromBody] CreateSpecialRequestTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateSpecialRequestTypeCommand(
                request.TenantId, request.Code, request.Name, request.Description,
                request.AddsProcesses, request.RemovesProcesses, request.OnlyProcesses),
            cancellationToken);
        return CreatedAtAction(nameof(GetSpecialRequestTypes), new { tenantId = result.TenantId }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateSpecialRequestType(Guid id, [FromBody] UpdateSpecialRequestTypeRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateSpecialRequestTypeCommand(
                id, request.Name, request.Description,
                request.AddsProcesses, request.RemovesProcesses, request.OnlyProcesses),
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateSpecialRequestType(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeactivateSpecialRequestTypeCommand(id), cancellationToken);
        return NoContent();
    }
}
