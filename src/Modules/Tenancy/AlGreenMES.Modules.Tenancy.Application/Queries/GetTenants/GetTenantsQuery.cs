using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Tenancy.Application.DTOs;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenants;

public record GetTenantsQuery : PagedQuery<PagedResult<TenantDto>>
{
    public bool? IsActive { get; init; }
}
