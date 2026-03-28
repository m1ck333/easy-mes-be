using AlGreenMES.Modules.Orders.Application.DTOs.Reports;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Reports.GetProcessAverages;

public record GetProcessAveragesQuery(Guid TenantId) : IRequest<ProcessAveragesDto>;
