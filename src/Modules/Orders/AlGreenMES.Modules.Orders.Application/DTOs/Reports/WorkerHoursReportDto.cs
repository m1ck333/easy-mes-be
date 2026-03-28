namespace AlGreenMES.Modules.Orders.Application.DTOs.Reports;

public record WorkerHoursReportDto(
    List<WorkerHoursSummaryDto> Workers);

public record WorkerHoursSummaryDto(
    Guid UserId,
    string FullName,
    int TotalMinutes,
    int SessionCount,
    List<WorkerHoursDayDto> DailyBreakdown);

public record WorkerHoursDayDto(
    DateOnly Date,
    int TotalMinutes,
    int SessionCount);
