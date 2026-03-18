using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletQueue;

public record GetTabletQueueQuery(Guid TenantId, Guid UserId) : IRequest<IReadOnlyList<ProcessGroupDto<TabletQueueItemDto>>>;
