using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletActiveWork;

public record GetTabletActiveWorkQuery(Guid ProcessId, Guid TenantId) : IRequest<IReadOnlyList<TabletActiveWorkDto>>;
