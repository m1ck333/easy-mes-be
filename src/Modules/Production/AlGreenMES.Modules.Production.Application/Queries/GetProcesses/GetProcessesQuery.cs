using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Application.DTOs;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProcesses;

public record GetProcessesQuery : PagedQuery<PagedResult<ProcessDto>>
{
    public Guid TenantId { get; init; }
    public bool? IsActive { get; init; }
}
