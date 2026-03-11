using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrderAttachments;

public record GetOrderAttachmentsQuery(Guid OrderId, Guid? OrderItemId = null) : IRequest<IReadOnlyList<OrderAttachmentDto>>;
