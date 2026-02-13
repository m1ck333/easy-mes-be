using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RemoveSpecialRequest;

public class RemoveSpecialRequestCommandHandler : IRequestHandler<RemoveSpecialRequestCommand, Unit>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public RemoveSpecialRequestCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveSpecialRequestCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.OrderId, cancellationToken);
        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        if (order.Status != OrderStatus.Draft)
            throw new DomainException("ORDER_NOT_DRAFT", "Can only remove special requests from draft orders.");

        var item = order.Items.FirstOrDefault(i => i.Id == request.OrderItemId);
        if (item == null)
            throw new NotFoundException("OrderItem", request.OrderItemId);

        var specialRequest = item.SpecialRequests.FirstOrDefault(sr => sr.Id == request.SpecialRequestId);
        if (specialRequest == null)
            throw new NotFoundException("SpecialRequest", request.SpecialRequestId);

        item.RemoveSpecialRequest(specialRequest.SpecialRequestTypeId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
