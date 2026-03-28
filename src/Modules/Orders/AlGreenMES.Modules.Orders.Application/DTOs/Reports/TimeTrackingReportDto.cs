namespace AlGreenMES.Modules.Orders.Application.DTOs.Reports;

public record TimeTrackingReportDto(
    List<TimeTrackingItemDto> Items,
    TimeTrackingSummaryDto Summary);

public record TimeTrackingItemDto(
    Guid OrderItemProcessId,
    string OrderNumber,
    string ProductName,
    Guid ProcessId,
    string ProcessCode,
    string ProcessName,
    string? Complexity,
    string Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    int TotalDurationMinutes);

public record TimeTrackingSummaryDto(
    int TotalRecords,
    double AvgDurationMinutes,
    int TotalDurationMinutes,
    int MinDurationMinutes,
    int MaxDurationMinutes);
