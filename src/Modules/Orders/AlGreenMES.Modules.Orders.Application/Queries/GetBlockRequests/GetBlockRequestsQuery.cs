using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetBlockRequests;

public record GetBlockRequestsQuery : PagedQuery<PagedResult<BlockRequestDto>>
{
    public Guid TenantId { get; init; }
    public RequestStatus? Status { get; init; }
    public Guid? OrderId { get; init; }
}
