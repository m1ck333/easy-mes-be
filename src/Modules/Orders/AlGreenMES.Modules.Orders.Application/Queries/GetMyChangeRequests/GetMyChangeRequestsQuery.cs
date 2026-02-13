using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetMyChangeRequests;

public record GetMyChangeRequestsQuery(Guid UserId) : IRequest<IReadOnlyList<ChangeRequestDto>>;
