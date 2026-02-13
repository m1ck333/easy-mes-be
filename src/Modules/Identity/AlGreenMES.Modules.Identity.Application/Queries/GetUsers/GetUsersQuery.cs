using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Entities;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetUsers;

public record GetUsersQuery : PagedQuery<PagedResult<UserDto>>
{
    public Guid TenantId { get; init; }
    public UserRole? Role { get; init; }
    public bool? IsActive { get; init; }
}
