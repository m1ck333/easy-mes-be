using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Enums;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrders;

public record GetOrdersQuery(Guid TenantId, OrderStatus? Status) : IRequest<IReadOnlyList<OrderDto>>;
