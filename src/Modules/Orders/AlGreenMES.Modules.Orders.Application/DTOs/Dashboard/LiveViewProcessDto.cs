namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record LiveViewProcessDto(
    Guid ProcessId,
    string ProcessCode,
    string ProcessName,
    List<LiveViewOrderDto> ActiveOrders,
    int QueueCount,
    int InProgressCount);
