using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletActiveWork;

public record GetTabletActiveWorkQuery(Guid TenantId, Guid UserId) : IRequest<IReadOnlyList<ProcessGroupDto<TabletActiveWorkDto>>>;
