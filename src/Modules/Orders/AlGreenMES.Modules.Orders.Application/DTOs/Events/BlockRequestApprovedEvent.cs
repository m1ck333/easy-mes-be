namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record BlockRequestApprovedEvent(
    Guid BlockRequestId,
    Guid? OrderItemProcessId,
    Guid? OrderItemSubProcessId,
    string BlockReason,
    Guid TenantId,
    Guid RequestedByUserId);
