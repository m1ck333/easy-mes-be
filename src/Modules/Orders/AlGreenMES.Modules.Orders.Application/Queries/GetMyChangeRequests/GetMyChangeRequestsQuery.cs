using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetMyChangeRequests;

public record GetMyChangeRequestsQuery : PagedQuery<PagedResult<ChangeRequestDto>>
{
    public Guid TenantId { get; init; }
    public Guid UserId { get; init; }
    public RequestStatus? Status { get; init; }
}
