using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderDetailDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public AddOrderItemCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDetailDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new DomainException("ORDER_NOT_FOUND", $"Order with id '{request.OrderId}' was not found.");

        order.AddItem(request.ProductCategoryId, request.ProductName, request.Quantity, request.Notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
