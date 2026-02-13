using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
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
            throw new NotFoundException("Order", request.OrderId);

        order.ChangePriority(request.Priority);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Adapt<OrderDto>();
    }
}
