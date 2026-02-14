using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, OrderDetailDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public AddOrderItemCommandHandler(IOrderRepository orderRepository, IOrdersUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderDetailDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order", request.OrderId);

        order.AddItem(request.ProductCategoryId, request.ProductName, request.Quantity, request.Notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Re-fetch with full details for a clean entity graph (the in-memory entity
        // has newly created items whose navigation properties are not fully populated)
        var refreshed = await _orderRepository.GetByIdWithFullDetailsAsync(request.OrderId, cancellationToken);
        return refreshed!.Adapt<OrderDetailDto>();
    }
}
