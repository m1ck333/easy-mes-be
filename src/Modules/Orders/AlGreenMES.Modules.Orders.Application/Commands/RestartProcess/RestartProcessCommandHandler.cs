using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RestartProcess;

public class RestartProcessCommandHandler : IRequestHandler<RestartProcessCommand, Unit>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public RestartProcessCommandHandler(
        IOrderItemProcessRepository processRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<Unit> Handle(RestartProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithFullDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new NotFoundException("OrderItemProcess", request.OrderItemProcessId);

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active.");

        process.Restart(request.ResetTime);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyProcessStartedAsync(new ProcessStartedEvent(
            process.Id, process.ProcessId, process.OrderItem.Order.Id,
            process.OrderItem.Order.OrderNumber, process.OrderItem.Order.TenantId), cancellationToken);

        return Unit.Value;
    }
}
