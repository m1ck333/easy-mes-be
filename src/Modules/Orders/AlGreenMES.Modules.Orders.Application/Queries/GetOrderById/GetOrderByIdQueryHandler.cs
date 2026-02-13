using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto?>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.Id, cancellationToken);
        if (order is null)
            return null;

        return new OrderDetailDto(
            order.Id,
            order.TenantId,
            order.OrderNumber,
            order.DeliveryDate,
            order.Priority,
            order.OrderType,
            order.Status,
            order.Notes,
            order.CustomWarningDays,
            order.CustomCriticalDays,
            order.Items.Select(i => new OrderItemDto(
                i.Id,
                i.OrderId,
                i.ProductCategoryId,
                i.ProductName,
                i.Quantity,
                i.Notes,
                i.Processes.Select(p => new OrderItemProcessDto(
                    p.Id,
                    p.OrderItemId,
                    p.ProcessId,
                    p.Complexity,
                    p.ComplexityOverridden,
                    p.Status,
                    p.StartedAt,
                    p.CompletedAt,
                    p.TotalDurationMinutes,
                    p.IsWithdrawn,
                    p.SubProcesses.Select(sp => new OrderItemSubProcessDto(
                        sp.Id,
                        sp.OrderItemProcessId,
                        sp.SubProcessId,
                        sp.Status,
                        sp.TotalDurationMinutes,
                        sp.IsWithdrawn)).ToList()
                )).ToList(),
                i.SpecialRequests.Select(sr => new OrderItemSpecialRequestDto(
                    sr.Id,
                    sr.SpecialRequestTypeId)).ToList()
            )).ToList());
    }
}
