namespace AlGreenMES.Modules.Orders.Api.Requests;

public record UpdateOrderRequest(
    string? Notes,
    int? CustomWarningDays,
    int? CustomCriticalDays);
