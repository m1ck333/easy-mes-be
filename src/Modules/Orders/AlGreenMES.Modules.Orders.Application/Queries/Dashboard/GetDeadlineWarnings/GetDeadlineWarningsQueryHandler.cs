using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetDeadlineWarnings;

public class GetDeadlineWarningsQueryHandler : IRequestHandler<GetDeadlineWarningsQuery, IReadOnlyList<DeadlineWarningDto>>
{
    private readonly IDashboardQueryService _dashboardQueryService;

    public GetDeadlineWarningsQueryHandler(IDashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    public async Task<IReadOnlyList<DeadlineWarningDto>> Handle(GetDeadlineWarningsQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardQueryService.GetDeadlineWarningsAsync(request.TenantId, cancellationToken);
    }
}
