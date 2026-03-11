using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.UploadOrderAttachment;

public record UploadOrderAttachmentCommand(
    Guid OrderId,
    Guid TenantId,
    Guid UserId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    Stream FileStream,
    Guid? OrderItemId = null) : IRequest<OrderAttachmentDto>;
