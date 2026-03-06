using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class BlockRequest : TenantEntity
{
    public Guid? OrderItemProcessId { get; private set; }
    public Guid? OrderItemSubProcessId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public string? RequestNote { get; private set; }
    public RequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public Guid? HandledByUserId { get; private set; }
    public DateTime? HandledAt { get; private set; }
    public string? BlockReason { get; private set; }
    public string? RejectionNote { get; private set; }

    public OrderItemProcess? OrderItemProcess { get; private set; }
    public OrderItemSubProcess? OrderItemSubProcess { get; private set; }

    private BlockRequest()
    {
    }

    public static BlockRequest CreateForProcess(Guid tenantId, Guid orderItemProcessId,
        Guid requestedByUserId, string? requestNote)
    {
        return new BlockRequest
        {
            TenantId = tenantId,
            OrderItemProcessId = orderItemProcessId,
            RequestedByUserId = requestedByUserId,
            RequestNote = requestNote?.Trim(),
            Status = RequestStatus.Pending
        };
    }

    public static BlockRequest CreateForSubProcess(Guid tenantId, Guid orderItemSubProcessId,
        Guid requestedByUserId, string? requestNote)
    {
        return new BlockRequest
        {
            TenantId = tenantId,
            OrderItemSubProcessId = orderItemSubProcessId,
            RequestedByUserId = requestedByUserId,
            RequestNote = requestNote?.Trim(),
            Status = RequestStatus.Pending
        };
    }

    public void Approve(Guid handledByUserId, string blockReason)
    {
        if (Status != RequestStatus.Pending)
            throw new DomainException("NOT_PENDING", "Only pending requests can be approved.");
        if (string.IsNullOrWhiteSpace(blockReason))
            throw new DomainException("REASON_REQUIRED", "Block reason is required.");

        Status = RequestStatus.Approved;
        HandledByUserId = handledByUserId;
        HandledAt = DateTime.UtcNow;
        BlockReason = blockReason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid handledByUserId, string? rejectionNote)
    {
        if (Status != RequestStatus.Pending)
            throw new DomainException("NOT_PENDING", "Only pending requests can be rejected.");

        Status = RequestStatus.Rejected;
        HandledByUserId = handledByUserId;
        HandledAt = DateTime.UtcNow;
        RejectionNote = rejectionNote?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
