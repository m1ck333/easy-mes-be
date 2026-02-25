using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.DeleteOrderAttachment;

public record DeleteOrderAttachmentCommand(
    Guid OrderId,
    Guid AttachmentId,
    Guid TenantId) : IRequest<Unit>;
