using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetDashboardStatistics;

public record GetDashboardStatisticsQuery(Guid TenantId) : IRequest<DashboardStatisticsDto>;
