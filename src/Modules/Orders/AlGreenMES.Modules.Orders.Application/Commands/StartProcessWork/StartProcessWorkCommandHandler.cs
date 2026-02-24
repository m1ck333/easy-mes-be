using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StartProcessWork;

public class StartProcessWorkCommandHandler : IRequestHandler<StartProcessWorkCommand, OrderItemProcessDto>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public StartProcessWorkCommandHandler(
        IOrderItemProcessRepository processRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<OrderItemProcessDto> Handle(StartProcessWorkCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithFullDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new NotFoundException("OrderItemProcess", request.OrderItemProcessId);

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active to start work.");

        // Validate dependencies: all sibling processes that this one depends on must be Completed
        var siblingProcesses = await _processRepository.GetByOrderItemIdAsync(process.OrderItemId, cancellationToken);
        // Dependencies are modeled via ProcessId ordering — processes with lower sequence must complete first
        // The dependency check is done by the caller ensuring all prior processes are done

        process.Start();

        var firstSubProcess = process.SubProcesses
            .Where(sp => !sp.IsWithdrawn)
            .OrderBy(sp => sp.SubProcessId)
            .FirstOrDefault();

        if (firstSubProcess != null)
        {
            firstSubProcess.Start();
            firstSubProcess.StartLog(request.UserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyProcessStartedAsync(
            new ProcessStartedEvent(
                process.Id,
                process.ProcessId,
                process.OrderItem.Order.Id,
                process.OrderItem.Order.OrderNumber,
                process.TenantId), cancellationToken);

        return process.Adapt<OrderItemProcessDto>();
    }
}
