namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record ProcessCompletedEvent(
    Guid OrderItemProcessId,
    Guid ProcessId,
    Guid OrderId,
    string OrderNumber,
    Guid TenantId);
