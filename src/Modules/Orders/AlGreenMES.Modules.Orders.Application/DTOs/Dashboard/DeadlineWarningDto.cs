namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record DeadlineWarningDto(
    Guid OrderId,
    string OrderNumber,
    DateTime DeliveryDate,
    int DaysRemaining,
    string Level,
    string Status,
    string? CurrentProcess);
