using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetByTenantIdAsync(request.TenantId, request.Status, cancellationToken);

        return orders.Select(o => new OrderDto(
            o.Id,
            o.TenantId,
            o.OrderNumber,
            o.DeliveryDate,
            o.Priority,
            o.OrderType,
            o.Status,
            o.Notes,
            o.CustomWarningDays,
            o.CustomCriticalDays,
            o.Items.Count)).ToList();
    }
}
