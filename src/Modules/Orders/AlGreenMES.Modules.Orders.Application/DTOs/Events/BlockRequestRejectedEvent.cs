namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record BlockRequestRejectedEvent(
    Guid BlockRequestId,
    Guid RequestedByUserId,
    string? RejectionNote,
    Guid TenantId);
