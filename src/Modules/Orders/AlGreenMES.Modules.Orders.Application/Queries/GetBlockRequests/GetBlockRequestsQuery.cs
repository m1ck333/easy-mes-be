using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetBlockRequests;

public record GetBlockRequestsQuery(Guid TenantId, RequestStatus? Status) : IRequest<IReadOnlyList<BlockRequestDto>>;
