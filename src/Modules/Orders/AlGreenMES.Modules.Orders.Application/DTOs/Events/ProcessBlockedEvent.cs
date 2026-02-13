namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record ProcessBlockedEvent(
    Guid OrderItemProcessId,
    Guid ProcessId,
    Guid OrderId,
    string OrderNumber,
    string Reason,
    Guid TenantId);
