namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record LiveViewOrderDto(
    Guid OrderId,
    string OrderNumber,
    Guid OrderItemId,
    string ProductName,
    string Status,
    bool IsBlocked,
    string? BlockReason);
