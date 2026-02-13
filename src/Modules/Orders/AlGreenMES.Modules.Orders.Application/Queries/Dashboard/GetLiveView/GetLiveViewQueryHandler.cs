using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetLiveView;

public class GetLiveViewQueryHandler : IRequestHandler<GetLiveViewQuery, IReadOnlyList<LiveViewProcessDto>>
{
    private readonly IDashboardQueryService _dashboardQueryService;

    public GetLiveViewQueryHandler(IDashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    public async Task<IReadOnlyList<LiveViewProcessDto>> Handle(GetLiveViewQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardQueryService.GetLiveViewAsync(request.TenantId, cancellationToken);
    }
}
