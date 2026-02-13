using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetWorkSessions;

public record GetWorkSessionsQuery : PagedQuery<PagedResult<WorkSessionDto>>
{
    public Guid TenantId { get; init; }
    public DateTime Date { get; init; }
    public Guid? UserId { get; init; }
}
