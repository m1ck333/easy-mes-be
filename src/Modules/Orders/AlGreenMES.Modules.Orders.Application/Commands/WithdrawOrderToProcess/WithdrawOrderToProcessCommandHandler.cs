using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.WithdrawOrderToProcess;

public class WithdrawOrderToProcessCommandHandler : IRequestHandler<WithdrawOrderToProcessCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public WithdrawOrderToProcessCommandHandler(
        IOrderRepository orderRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<Unit> Handle(WithdrawOrderToProcessCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new DomainException("ORDER_NOT_FOUND", $"Order with id '{request.OrderId}' was not found.");

        if (order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active to withdraw to a process.");

        foreach (var item in order.Items)
        {
            var targetProcess = item.Processes.FirstOrDefault(p => p.ProcessId == request.TargetProcessId);
            if (targetProcess == null) continue;

            // Withdraw processes that are at or after the target process and not already completed/withdrawn
            foreach (var process in item.Processes)
            {
                if (process.ProcessId == request.TargetProcessId ||
                    process.Status == ProcessStatus.InProgress ||
                    process.Status == ProcessStatus.Pending)
                {
                    if (process.Status != ProcessStatus.Completed &&
                        process.Status != ProcessStatus.Withdrawn)
                    {
                        process.Withdraw(request.UserId, request.Reason);
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
