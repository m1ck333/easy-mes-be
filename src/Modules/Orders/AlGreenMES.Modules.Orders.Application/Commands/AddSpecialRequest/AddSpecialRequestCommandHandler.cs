using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.AddSpecialRequest;

public class AddSpecialRequestCommandHandler : IRequestHandler<AddSpecialRequestCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public AddSpecialRequestCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AddSpecialRequestCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.Status != OrderStatus.Draft)
            throw new DomainException("ORDER_NOT_DRAFT", "Can only add special requests to draft orders.");

        var item = order.Items.FirstOrDefault(i => i.Id == request.OrderItemId);
        if (item == null)
            throw new NotFoundException("OrderItem", request.OrderItemId);

        item.AddSpecialRequest(request.SpecialRequestTypeId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
