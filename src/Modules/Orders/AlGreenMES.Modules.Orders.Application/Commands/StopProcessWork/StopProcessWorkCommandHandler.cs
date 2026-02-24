using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StopProcessWork;

public class StopProcessWorkCommandHandler : IRequestHandler<StopProcessWorkCommand, Unit>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public StopProcessWorkCommandHandler(
        IOrderItemProcessRepository processRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(StopProcessWorkCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithFullDetailsAsync(request.OrderItemProcessId, cancellationToken);
        if (process == null)
            throw new NotFoundException("OrderItemProcess", request.OrderItemProcessId);

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active.");

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
