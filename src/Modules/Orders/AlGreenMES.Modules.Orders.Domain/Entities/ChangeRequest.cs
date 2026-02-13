using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class ChangeRequest : TenantEntity
{
    public Guid OrderId { get; private set; }
    public Guid RequestedByUserId { get; private set; }
    public ChangeRequestType RequestType { get; private set; }
    public string Description { get; private set; } = null!;
    public RequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public Guid? HandledByUserId { get; private set; }
    public DateTime? HandledAt { get; private set; }
    public string? ResponseNote { get; private set; }

    public Order Order { get; private set; } = null!;

    private ChangeRequest()
    {
    }

    public static ChangeRequest Create(Guid tenantId, Guid orderId, Guid requestedByUserId,
        ChangeRequestType requestType, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("DESCRIPTION_REQUIRED", "Description is required.");

        return new ChangeRequest
        {
            TenantId = tenantId,
            OrderId = orderId,
            RequestedByUserId = requestedByUserId,
            RequestType = requestType,
            Description = description.Trim(),
            Status = RequestStatus.Pending
        };
    }

    public void Approve(Guid handledByUserId, string? responseNote)
    {
        if (Status != RequestStatus.Pending)
            throw new DomainException("NOT_PENDING", "Only pending requests can be approved.");

        Status = RequestStatus.Approved;
        HandledByUserId = handledByUserId;
        HandledAt = DateTime.UtcNow;
        ResponseNote = responseNote?.Trim();
    }

    public void Reject(Guid handledByUserId, string? responseNote)
    {
        if (Status != RequestStatus.Pending)
            throw new DomainException("NOT_PENDING", "Only pending requests can be rejected.");

        Status = RequestStatus.Rejected;
        HandledByUserId = handledByUserId;
        HandledAt = DateTime.UtcNow;
        ResponseNote = responseNote?.Trim();
    }
}
