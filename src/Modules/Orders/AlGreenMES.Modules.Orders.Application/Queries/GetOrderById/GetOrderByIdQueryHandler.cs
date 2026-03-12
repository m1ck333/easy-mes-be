using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderAttachmentRepository _attachmentRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IOrderAttachmentRepository attachmentRepository)
    {
        _orderRepository = orderRepository;
        _attachmentRepository = attachmentRepository;
    }

    public async Task<OrderDetailDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        var attachments = await _attachmentRepository.GetByOrderIdAsync(request.Id, cancellationToken);

        var dto = order.Adapt<OrderDetailDto>();

        // Set order-level attachments
        var orderLevelAttachments = attachments
            .Where(a => a.OrderItemId == null)
            .Select(a => a.Adapt<OrderAttachmentDto>())
            .ToList();

        // Distribute item-level attachments to each item DTO
        var itemAttachments = attachments
            .Where(a => a.OrderItemId != null)
            .GroupBy(a => a.OrderItemId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(a => a.Adapt<OrderAttachmentDto>()).ToList());

        var updatedItems = dto.Items.Select(item =>
            item with { Attachments = itemAttachments.GetValueOrDefault(item.Id, []) }
        ).ToList();

        return dto with { Items = updatedItems, Attachments = orderLevelAttachments };
    }
}
