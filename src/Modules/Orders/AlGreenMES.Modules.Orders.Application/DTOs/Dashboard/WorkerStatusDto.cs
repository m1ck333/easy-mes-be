namespace AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;

public record WorkerDto(Guid UserId, string Name, DateTime CheckedInAt);

public record WorkerStatusDto(
    Guid ProcessId,
    string ProcessCode,
    string ProcessName,
    bool IsWorkerCheckedIn,
    WorkerDto? Worker);
