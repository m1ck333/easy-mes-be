using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ChangePriority;

public class ChangePriorityCommandHandler : IRequestHandler<ChangePriorityCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public ChangePriorityCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDto> Handle(ChangePriorityCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new DomainException("ORDER_NOT_FOUND", $"Order with id '{request.OrderId}' was not found.");

        order.ChangePriority(request.Priority);
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
