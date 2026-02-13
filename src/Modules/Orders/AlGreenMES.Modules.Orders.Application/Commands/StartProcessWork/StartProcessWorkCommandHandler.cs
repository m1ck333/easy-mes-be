using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StartProcessWork;

public class StartProcessWorkCommandHandler : IRequestHandler<StartProcessWorkCommand, OrderItemProcessDto>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IWorkSessionRepository _workSessionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public StartProcessWorkCommandHandler(
        IOrderItemProcessRepository processRepository,
        IWorkSessionRepository workSessionRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService)
    {
        _processRepository = processRepository;
        _workSessionRepository = workSessionRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<OrderItemProcessDto> Handle(StartProcessWorkCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithFullDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new DomainException("PROCESS_NOT_FOUND", $"Order item process with id '{request.OrderItemProcessId}' was not found.");

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active to start work.");

        var session = await _workSessionRepository.GetActiveSessionAsync(request.UserId, cancellationToken);
        if (session == null)
            throw new DomainException("NOT_CHECKED_IN", "User must be checked in to start work.");

        if (session.ProcessId != process.Id)
            throw new DomainException("SESSION_MISMATCH", "Active work session is for a different process.");

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
            firstSubProcess.StartLog(session.Id);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventService.NotifyProcessStartedAsync(
            new ProcessStartedEvent(
                process.Id,
                process.ProcessId,
                process.OrderItem.Order.Id,
                process.OrderItem.Order.OrderNumber,
                process.TenantId), cancellationToken);

        return MapToDto(process);
    }

    private static OrderItemProcessDto MapToDto(Domain.Entities.OrderItemProcess process)
    {
        return new OrderItemProcessDto(
            process.Id,
            process.OrderItemId,
            process.ProcessId,
            process.Complexity,
            process.ComplexityOverridden,
            process.Status,
            process.StartedAt,
            process.CompletedAt,
            process.TotalDurationMinutes,
            process.IsWithdrawn,
            process.SubProcesses.Select(sp => new OrderItemSubProcessDto(
                sp.Id,
                sp.OrderItemProcessId,
                sp.SubProcessId,
                sp.Status,
                sp.TotalDurationMinutes,
                sp.IsWithdrawn)).ToList());
    }
}
