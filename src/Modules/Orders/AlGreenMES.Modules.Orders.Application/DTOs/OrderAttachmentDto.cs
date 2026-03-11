namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record OrderAttachmentDto(
    Guid Id,
    Guid OrderId,
    Guid? OrderItemId,
    string OriginalFileName,
    string ContentType,
    long FileSizeBytes,
    DateTime UploadedAt);
