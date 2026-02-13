using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.Id, cancellationToken)
            ?? throw new DomainException("ORDER_NOT_FOUND", $"Order with id '{request.Id}' was not found.");

        order.Update(request.Notes, request.CustomWarningDays, request.CustomCriticalDays);
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
            order.Items.Count);
    }
}
