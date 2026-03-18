using AlGreenMES.Modules.Identity.Api.Requests;
using AlGreenMES.Modules.Identity.Application.Commands.ChangePassword;
using AlGreenMES.Modules.Identity.Application.Commands.CreateUser;
using AlGreenMES.Modules.Identity.Application.Commands.ResetPassword;
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
        [FromQuery] DateTime? createdFrom = null,
        [FromQuery] DateTime? createdTo = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUsersQuery
        {
            TenantId = tenantId,
            Role = role,
            IsActive = isActive,
            Page = page,
            PageSize = pageSize,
            Search = search,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo
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
                request.ProcessIds),
            cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateUserCommand(id, request.TenantId, request.FirstName, request.LastName, request.Role, request.IsActive, request.CanIncludeWithdrawnInAnalysis, request.ProcessIds),
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

    [HttpPost("{id:guid}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ResetPasswordCommand(id, request.NewPassword),
            cancellationToken);

        return NoContent();
    }
}
