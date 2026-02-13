using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StopProcessWork;

public class StopProcessWorkCommandHandler : IRequestHandler<StopProcessWorkCommand, Unit>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IWorkSessionRepository _workSessionRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public StopProcessWorkCommandHandler(
        IOrderItemProcessRepository processRepository,
        IWorkSessionRepository workSessionRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _workSessionRepository = workSessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(StopProcessWorkCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithFullDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new DomainException("PROCESS_NOT_FOUND", $"Order item process with id '{request.OrderItemProcessId}' was not found.");

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active.");

        var session = await _workSessionRepository.GetActiveSessionAsync(request.UserId, cancellationToken);
        if (session == null)
            throw new DomainException("NOT_CHECKED_IN", "User must be checked in.");

        if (session.ProcessId != process.Id)
            throw new DomainException("SESSION_MISMATCH", "Active work session is for a different process.");

        if (process.Status != ProcessStatus.InProgress)
            throw new DomainException("INVALID_STATUS", "Process must be in progress to stop work.");

        var activeSubProcess = process.SubProcesses
            .FirstOrDefault(sp => sp.Status == SubProcessStatus.InProgress);

        if (activeSubProcess != null)
        {
            var openLog = activeSubProcess.GetOpenLog();
            if (openLog != null)
            {
                openLog.End();
                if (openLog.DurationMinutes.HasValue)
                    activeSubProcess.AddDuration(openLog.DurationMinutes.Value);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
