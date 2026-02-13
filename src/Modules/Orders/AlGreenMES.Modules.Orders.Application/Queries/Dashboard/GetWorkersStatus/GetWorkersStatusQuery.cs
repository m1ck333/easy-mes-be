using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetWorkersStatus;

public record GetWorkersStatusQuery(Guid TenantId) : IRequest<IReadOnlyList<WorkerStatusDto>>;
