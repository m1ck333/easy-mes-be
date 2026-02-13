using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletQueue;

public record GetTabletQueueQuery(Guid ProcessId, Guid TenantId) : IRequest<IReadOnlyList<TabletQueueItemDto>>;
