using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Application.DTOs;

namespace AlGreenMES.Modules.Production.Application.Queries.GetSpecialRequestTypes;

public record GetSpecialRequestTypesQuery : PagedQuery<PagedResult<SpecialRequestTypeDto>>
{
    public Guid TenantId { get; init; }
    public bool? IsActive { get; init; }
}
