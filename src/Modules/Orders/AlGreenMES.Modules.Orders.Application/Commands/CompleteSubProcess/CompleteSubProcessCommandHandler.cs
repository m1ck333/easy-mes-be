using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CompleteSubProcess;

public class CompleteSubProcessCommandHandler : IRequestHandler<CompleteSubProcessCommand, OrderItemSubProcessDto>
{
    private readonly IOrderItemSubProcessRepository _subProcessRepository;
    private readonly IWorkSessionRepository _workSessionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;
    private readonly IProductionEventService _eventService;

    public CompleteSubProcessCommandHandler(
        IOrderItemSubProcessRepository subProcessRepository,
        IWorkSessionRepository workSessionRepository,
        IOrdersUnitOfWork unitOfWork,
        IProductionEventService eventService)
    {
        _subProcessRepository = subProcessRepository;
        _workSessionRepository = workSessionRepository;
        _unitOfWork = unitOfWork;
        _eventService = eventService;
    }

    public async Task<OrderItemSubProcessDto> Handle(CompleteSubProcessCommand request, CancellationToken cancellationToken)
    {
        var subProcess = await _subProcessRepository.GetByIdWithFullDetailsAsync(request.OrderItemSubProcessId, cancellationToken);
        if (subProcess == null)
            throw new DomainException("SUBPROCESS_NOT_FOUND", $"Sub-process with id '{request.OrderItemSubProcessId}' was not found.");

        var process = subProcess.OrderItemProcess;

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active.");

        var session = await _workSessionRepository.GetActiveSessionAsync(request.UserId, cancellationToken);
        if (session == null)
            throw new DomainException("NOT_CHECKED_IN", "User must be checked in.");

        if (session.ProcessId != process.Id)
            throw new DomainException("SESSION_MISMATCH", "Active work session is for a different process.");

        if (subProcess.Status != SubProcessStatus.InProgress)
            throw new DomainException("INVALID_STATUS", "Sub-process must be in progress to complete.");

        // End current open log
        var openLog = subProcess.GetOpenLog();
        if (openLog != null)
        {
            openLog.End();
            if (openLog.DurationMinutes.HasValue)
                subProcess.AddDuration(openLog.DurationMinutes.Value);
        }

        subProcess.Complete();

        // Check if all sub-processes are done → auto-complete parent process
        var allCompleteOrWithdrawn = process.SubProcesses.All(sp =>
            sp.Status == SubProcessStatus.Completed || sp.Status == SubProcessStatus.Withdrawn);

        bool parentCompleted = false;
        if (allCompleteOrWithdrawn)
        {
            var totalDuration = process.SubProcesses
                .Where(sp => sp.Status == SubProcessStatus.Completed)
                .Sum(sp => sp.TotalDurationMinutes);

            var delta = totalDuration - process.TotalDurationMinutes;
            if (delta > 0)
                process.AddDuration(delta);

            process.Complete();
            parentCompleted = true;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (parentCompleted)
        {
            await _eventService.NotifyProcessCompletedAsync(
                new ProcessCompletedEvent(
                    process.Id,
                    process.ProcessId,
                    process.OrderItem.Order.Id,
                    process.OrderItem.Order.OrderNumber,
                    process.TenantId), cancellationToken);
        }

        return new OrderItemSubProcessDto(
            subProcess.Id,
            subProcess.OrderItemProcessId,
            subProcess.SubProcessId,
            subProcess.Status,
            subProcess.TotalDurationMinutes,
            subProcess.IsWithdrawn);
    }
}
