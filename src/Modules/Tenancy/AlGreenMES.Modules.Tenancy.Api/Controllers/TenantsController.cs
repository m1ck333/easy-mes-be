using AlGreenMES.Modules.Tenancy.Api.Requests;
using AlGreenMES.Modules.Tenancy.Application.Commands.CreateTenant;
using AlGreenMES.Modules.Tenancy.Application.Commands.UpdateTenant;
using AlGreenMES.Modules.Tenancy.Application.Commands.UpdateTenantSettings;
using AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantById;
using AlGreenMES.Modules.Tenancy.Application.Queries.GetTenants;
using AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Tenancy.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTenants(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTenantsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTenantById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTenantByIdQuery(id), cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateTenantCommand(request.Name, request.Code), cancellationToken);
        return CreatedAtAction(nameof(GetTenantById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTenant(Guid id, [FromBody] UpdateTenantRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTenantCommand(id, request.Name, request.IsActive), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/settings")]
    public async Task<IActionResult> GetTenantSettings(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTenantSettingsQuery(id), cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{id:guid}/settings")]
    public async Task<IActionResult> UpdateTenantSettings(Guid id, [FromBody] UpdateTenantSettingsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTenantSettingsCommand(
            id,
            request.DefaultWarningDays,
            request.DefaultCriticalDays,
            request.WarningColor,
            request.CriticalColor), cancellationToken);

        return Ok(result);
    }
}
