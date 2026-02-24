using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StartSubProcess;

public class StartSubProcessCommandHandler : IRequestHandler<StartSubProcessCommand, OrderItemSubProcessDto>
{
    private readonly IOrderItemSubProcessRepository _subProcessRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public StartSubProcessCommandHandler(
        IOrderItemSubProcessRepository subProcessRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _subProcessRepository = subProcessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderItemSubProcessDto> Handle(StartSubProcessCommand request, CancellationToken cancellationToken)
    {
        var subProcess = await _subProcessRepository.GetByIdWithFullDetailsAsync(request.OrderItemSubProcessId, cancellationToken);
        if (subProcess == null)
            throw new NotFoundException("OrderItemSubProcess", request.OrderItemSubProcessId);

        var process = subProcess.OrderItemProcess;

        if (process.OrderItem.Order.Status != OrderStatus.Active)
            throw new DomainException("ORDER_NOT_ACTIVE", "Order must be active to start a sub-process.");

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
        subProcess.StartLog(request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subProcess.Adapt<OrderItemSubProcessDto>();
    }
}
