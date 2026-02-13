using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ActivateOrder;

public class ActivateOrderCommandHandler : IRequestHandler<ActivateOrderCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public ActivateOrderCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork, IProductionEventService eventService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<Unit> Handle(ActivateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        order.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyOrderActivatedAsync(
            new OrderActivatedEvent(order.Id, order.OrderNumber, order.TenantId), cancellationToken);

        return Unit.Value;
    }
}
