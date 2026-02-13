namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record DeadlineWarningEvent(
    Guid OrderId,
    string OrderNumber,
    DateTime DeliveryDate,
    int DaysRemaining,
    string Level,
    Guid TenantId);
