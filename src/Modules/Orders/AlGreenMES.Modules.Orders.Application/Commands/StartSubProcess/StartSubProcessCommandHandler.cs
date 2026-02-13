using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StartSubProcess;

public class StartSubProcessCommandHandler : IRequestHandler<StartSubProcessCommand, OrderItemSubProcessDto>
{
    private readonly IOrderItemSubProcessRepository _subProcessRepository;
    private readonly IWorkSessionRepository _workSessionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public StartSubProcessCommandHandler(
        IOrderItemSubProcessRepository subProcessRepository,
        IWorkSessionRepository workSessionRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _subProcessRepository = subProcessRepository;
        _workSessionRepository = workSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderItemSubProcessDto> Handle(StartSubProcessCommand request, CancellationToken cancellationToken)
    {
        var subProcess = await _subProcessRepository.GetByIdWithFullDetailsAsync(request.OrderItemSubProcessId, cancellationToken);
        if (subProcess == null)
            throw new DomainException("SUBPROCESS_NOT_FOUND", $"Sub-process with id '{request.OrderItemSubProcessId}' was not found.");

        var process = subProcess.OrderItemProcess;

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active to start a sub-process.");

        var session = await _workSessionRepository.GetActiveSessionAsync(request.UserId, cancellationToken);
        if (session == null)
            throw new DomainException("NOT_CHECKED_IN", "User must be checked in.");

        if (session.ProcessId != process.Id)
            throw new DomainException("SESSION_MISMATCH", "Active work session is for a different process.");

        if (process.Status != ProcessStatus.InProgress)
            throw new DomainException("PROCESS_NOT_STARTED", "Parent process must be in progress.");

        // Validate strict order: all previous sub-processes must be Completed
        var siblingSubProcesses = process.SubProcesses
            .Where(sp => !sp.IsWithdrawn)
            .OrderBy(sp => sp.SubProcessId)
            .ToList();

        var currentIndex = siblingSubProcesses.FindIndex(sp => sp.Id == subProcess.Id);
        for (int i = 0; i < currentIndex; i++)
        {
            if (siblingSubProcesses[i].Status != SubProcessStatus.Completed)
                throw new DomainException("PREVIOUS_NOT_COMPLETED",
                    "Previous sub-process must be completed before starting this one.");
        }

        subProcess.Start();
        subProcess.StartLog(session.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderItemSubProcessDto(
            subProcess.Id,
            subProcess.OrderItemProcessId,
            subProcess.SubProcessId,
            subProcess.Status,
            subProcess.TotalDurationMinutes,
            subProcess.IsWithdrawn);
    }
}
