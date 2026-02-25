using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class OrderAttachment : TenantEntity
{
    public Guid OrderId { get; private set; }
    public string OriginalFileName { get; private set; } = null!;
    public string StoredFileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public string StoragePath { get; private set; } = null!;
    public DateTime UploadedAt { get; private set; }
    public Guid UploadedByUserId { get; private set; }

    public Order Order { get; private set; } = null!;

    private OrderAttachment() { }

    public static OrderAttachment Create(
        Guid tenantId,
        Guid orderId,
        string originalFileName,
        string storedFileName,
        string contentType,
        long fileSizeBytes,
        string storagePath,
        Guid uploadedByUserId)
    {
        if (string.IsNullOrWhiteSpace(originalFileName))
            throw new DomainException("INVALID_FILE_NAME", "File name is required.");
        if (fileSizeBytes <= 0)
            throw new DomainException("INVALID_FILE_SIZE", "File size must be positive.");

        return new OrderAttachment
        {
            TenantId = tenantId,
            OrderId = orderId,
            OriginalFileName = originalFileName.Trim(),
            StoredFileName = storedFileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            StoragePath = storagePath,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId = uploadedByUserId
        };
    }
}
