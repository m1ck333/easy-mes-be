using AlGreenMES.Modules.Orders.Application.DTOs.Reports;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Reports.GetWorkerHours;

public record GetWorkerHoursQuery(
    Guid TenantId,
    DateOnly From,
    DateOnly To,
    Guid? UserId) : IRequest<WorkerHoursReportDto>;
