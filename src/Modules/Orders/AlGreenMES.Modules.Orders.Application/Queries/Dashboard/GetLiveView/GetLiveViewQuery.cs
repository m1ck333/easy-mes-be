using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetLiveView;

public record GetLiveViewQuery(Guid TenantId) : IRequest<IReadOnlyList<LiveViewProcessDto>>;
