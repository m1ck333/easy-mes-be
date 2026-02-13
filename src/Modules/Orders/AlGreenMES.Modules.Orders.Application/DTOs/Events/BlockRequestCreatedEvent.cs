namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record BlockRequestCreatedEvent(
    Guid BlockRequestId,
    Guid? OrderItemProcessId,
    Guid? OrderItemSubProcessId,
    string? RequestNote,
    Guid TenantId);
