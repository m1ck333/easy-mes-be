using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.UnblockProcess;

public class UnblockProcessCommandHandler : IRequestHandler<UnblockProcessCommand, Unit>
{
    private readonly IOrderItemProcessRepository _orderItemProcessRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public UnblockProcessCommandHandler(IOrderItemProcessRepository orderItemProcessRepository, IOrdersUnitOfWork unitOfWork, IProductionEventService eventService)
    {
        _orderItemProcessRepository = orderItemProcessRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<Unit> Handle(UnblockProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _orderItemProcessRepository.GetByIdWithOrderDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new NotFoundException("OrderItemProcess", request.OrderItemProcessId);

        process.Unblock(request.UserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyProcessUnblockedAsync(
            new ProcessUnblockedEvent(
                process.Id,
                process.ProcessId,
                process.OrderItem.Order.Id,
                process.OrderItem.Order.OrderNumber,
                process.TenantId), cancellationToken);

        return Unit.Value;
    }
}
