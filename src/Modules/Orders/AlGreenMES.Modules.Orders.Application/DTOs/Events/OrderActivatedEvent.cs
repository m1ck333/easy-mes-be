namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record OrderActivatedEvent(
    Guid OrderId,
    string OrderNumber,
    Guid TenantId);
