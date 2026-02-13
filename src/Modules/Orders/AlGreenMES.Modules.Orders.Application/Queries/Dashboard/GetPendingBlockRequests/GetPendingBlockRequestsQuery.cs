using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetPendingBlockRequests;

public record GetPendingBlockRequestsQuery(Guid TenantId) : IRequest<IReadOnlyList<PendingBlockRequestDto>>;
