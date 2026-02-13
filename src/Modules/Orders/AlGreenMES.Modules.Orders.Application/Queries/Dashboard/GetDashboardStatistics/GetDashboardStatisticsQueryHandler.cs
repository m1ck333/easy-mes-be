using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetDashboardStatistics;

public class GetDashboardStatisticsQueryHandler : IRequestHandler<GetDashboardStatisticsQuery, DashboardStatisticsDto>
{
    private readonly IDashboardQueryService _dashboardQueryService;

    public GetDashboardStatisticsQueryHandler(IDashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    public async Task<DashboardStatisticsDto> Handle(GetDashboardStatisticsQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardQueryService.GetDashboardStatisticsAsync(request.TenantId, cancellationToken);
    }
}
