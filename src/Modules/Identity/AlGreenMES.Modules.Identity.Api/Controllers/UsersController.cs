using AlGreenMES.Modules.Identity.Api.Requests;
using AlGreenMES.Modules.Identity.Application.Commands.ChangePassword;
using AlGreenMES.Modules.Identity.Application.Commands.CreateUser;
using AlGreenMES.Modules.Identity.Application.Commands.UpdateUser;
using AlGreenMES.Modules.Identity.Application.Queries.GetUserById;
using AlGreenMES.Modules.Identity.Application.Queries.GetUsers;
using AlGreenMES.Modules.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlGreenMES.Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] Guid tenantId,
        [FromQuery] UserRole? role,
        [FromQuery] bool? isActive,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUsersQuery
        {
            TenantId = tenantId,
            Role = role,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize,
            Search = search
        }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateUserCommand(
                request.TenantId,
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName,
                request.Role,
                request.ProcessId),
            cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateUserCommand(id, request.FirstName, request.LastName, request.Role, request.IsActive, request.CanIncludeWithdrawnInAnalysis, request.ProcessId),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword),
            cancellationToken);

        return NoContent();
    }
}
