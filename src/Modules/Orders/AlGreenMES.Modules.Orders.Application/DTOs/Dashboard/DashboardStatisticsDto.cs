namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record DashboardTodayDto(
    int OrdersCompleted,
    int OrdersActive,
    int ProcessesCompleted,
    double AverageProcessTimeMinutes);

public record DashboardWarningsDto(int CriticalCount, int WarningCount);

public record DashboardStatisticsDto(
    DashboardTodayDto Today,
    DashboardWarningsDto Warnings,
    int PendingBlockRequests);
