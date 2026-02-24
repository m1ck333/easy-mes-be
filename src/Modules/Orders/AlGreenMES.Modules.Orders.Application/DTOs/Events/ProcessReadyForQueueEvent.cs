namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record ProcessReadyForQueueEvent(
    Guid OrderItemProcessId,
    Guid ProcessId,
    Guid OrderId,
    string OrderNumber,
    Guid TenantId);
