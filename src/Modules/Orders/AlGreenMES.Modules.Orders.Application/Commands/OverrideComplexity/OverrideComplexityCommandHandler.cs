using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.OverrideComplexity;

public class OverrideComplexityCommandHandler : IRequestHandler<OverrideComplexityCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public OverrideComplexityCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(OverrideComplexityCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new DomainException("ORDER_NOT_FOUND", $"Order with id '{request.OrderId}' was not found.");

        if (order.Status != OrderStatus.Draft)
            throw new DomainException("ORDER_NOT_DRAFT", "Can only override complexity on draft orders.");

        var item = order.Items.FirstOrDefault(i => i.Id == request.OrderItemId);
        if (item == null)
            throw new DomainException("ITEM_NOT_FOUND", $"Order item with id '{request.OrderItemId}' was not found.");

        var process = item.Processes.FirstOrDefault(p => p.Id == request.OrderItemProcessId);
        if (process == null)
            throw new DomainException("PROCESS_NOT_FOUND", $"Process with id '{request.OrderItemProcessId}' was not found.");

        process.OverrideComplexity(request.Complexity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
