using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetChangeRequests;

public record GetChangeRequestsQuery(Guid TenantId, RequestStatus? Status) : IRequest<IReadOnlyList<ChangeRequestDto>>;
