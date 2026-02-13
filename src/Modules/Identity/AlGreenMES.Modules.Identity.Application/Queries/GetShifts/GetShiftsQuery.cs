using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Identity.Application.DTOs;

namespace AlGreenMES.Modules.Identity.Application.Queries.GetShifts;

public record GetShiftsQuery : PagedQuery<PagedResult<ShiftDto>>
{
    public Guid TenantId { get; init; }
    public bool? IsActive { get; init; }
}
