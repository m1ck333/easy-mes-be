namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record PendingBlockRequestDto(
    Guid Id,
    string OrderNumber,
    string ProcessName,
    string ProductName,
    string RequestedBy,
    DateTime RequestedAt,
    string? RequestNote);
