using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetWorkSessions;

public record GetWorkSessionsQuery(Guid TenantId, DateTime Date) : IRequest<IReadOnlyList<WorkSessionDto>>;
