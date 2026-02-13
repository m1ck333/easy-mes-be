using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetChangeRequests;

public record GetChangeRequestsQuery : PagedQuery<PagedResult<ChangeRequestDto>>
{
    public Guid TenantId { get; init; }
    public RequestStatus? Status { get; init; }
    public ChangeRequestType? RequestType { get; init; }
}
