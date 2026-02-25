using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrderAttachments;

public record GetOrderAttachmentsQuery(Guid OrderId) : IRequest<IReadOnlyList<OrderAttachmentDto>>;
