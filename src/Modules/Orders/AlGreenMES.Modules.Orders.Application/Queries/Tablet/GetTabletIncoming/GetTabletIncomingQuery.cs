using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletIncoming;

public record GetTabletIncomingQuery(Guid TenantId, Guid UserId) : IRequest<IReadOnlyList<ProcessGroupDto<TabletIncomingDto>>>;
