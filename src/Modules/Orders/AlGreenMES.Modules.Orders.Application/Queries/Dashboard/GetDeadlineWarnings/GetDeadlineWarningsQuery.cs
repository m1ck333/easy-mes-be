using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Dashboard.GetDeadlineWarnings;

public record GetDeadlineWarningsQuery(Guid TenantId) : IRequest<IReadOnlyList<DeadlineWarningDto>>;
