using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var exists = await _orderRepository.ExistsByOrderNumberAsync(request.OrderNumber, request.TenantId, cancellationToken);
        if (exists)
            throw new DomainException("ORDER_NUMBER_EXISTS", $"An order with number '{request.OrderNumber}' already exists.");

        var order = Order.Create(
            request.TenantId,
            request.OrderNumber,
            request.DeliveryDate,
            request.Priority,
            request.OrderType,
            request.CreatedByUserId,
            request.Notes);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderDto(
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
            0);
    }
}
