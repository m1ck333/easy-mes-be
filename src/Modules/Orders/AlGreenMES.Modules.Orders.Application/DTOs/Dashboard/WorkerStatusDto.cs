namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record WorkerStatusDto(
    Guid UserId,
    string Name,
    bool IsCheckedIn,
    DateTime? CheckedInAt,
    List<string> AssignedProcessCodes);
