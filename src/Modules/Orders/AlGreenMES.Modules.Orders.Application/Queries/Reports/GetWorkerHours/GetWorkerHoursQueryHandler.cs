using AlGreenMES.Modules.Orders.Application.DTOs.Reports;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Reports.GetWorkerHours;

public class GetWorkerHoursQueryHandler : IRequestHandler<GetWorkerHoursQuery, WorkerHoursReportDto>
{
    private readonly IReportingQueryService _reportingQueryService;

    public GetWorkerHoursQueryHandler(IReportingQueryService reportingQueryService)
    {
        _reportingQueryService = reportingQueryService;
    }

    public async Task<WorkerHoursReportDto> Handle(GetWorkerHoursQuery request, CancellationToken cancellationToken)
    {
        return await _reportingQueryService.GetWorkerHoursReportAsync(
            request.TenantId, request.From, request.To, request.UserId, cancellationToken);
    }
}
