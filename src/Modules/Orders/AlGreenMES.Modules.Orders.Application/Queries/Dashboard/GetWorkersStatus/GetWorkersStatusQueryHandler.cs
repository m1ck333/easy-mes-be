using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetWorkersStatus;

public class GetWorkersStatusQueryHandler : IRequestHandler<GetWorkersStatusQuery, IReadOnlyList<WorkerStatusDto>>
{
    private readonly IDashboardQueryService _dashboardQueryService;

    public GetWorkersStatusQueryHandler(IDashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    public async Task<IReadOnlyList<WorkerStatusDto>> Handle(GetWorkersStatusQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardQueryService.GetWorkersStatusAsync(request.TenantId, cancellationToken);
    }
}
