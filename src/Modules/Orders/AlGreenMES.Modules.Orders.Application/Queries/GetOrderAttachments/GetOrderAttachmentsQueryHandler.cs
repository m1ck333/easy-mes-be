using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrderAttachments;

public class GetOrderAttachmentsQueryHandler : IRequestHandler<GetOrderAttachmentsQuery, IReadOnlyList<OrderAttachmentDto>>
{
    private readonly IOrderAttachmentRepository _attachmentRepository;

    public GetOrderAttachmentsQueryHandler(IOrderAttachmentRepository attachmentRepository)
    {
        _attachmentRepository = attachmentRepository;
    }

    public async Task<IReadOnlyList<OrderAttachmentDto>> Handle(GetOrderAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var attachments = request.OrderItemId.HasValue
            ? await _attachmentRepository.GetByOrderItemIdAsync(request.OrderItemId.Value, cancellationToken)
            : await _attachmentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        return attachments.Adapt<IReadOnlyList<OrderAttachmentDto>>();
    }
}
