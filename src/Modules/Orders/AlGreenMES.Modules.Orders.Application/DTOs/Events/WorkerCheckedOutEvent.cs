namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record WorkerCheckedOutEvent(
    Guid UserId,
    Guid ProcessId,
    Guid SessionId,
    int? DurationMinutes,
    Guid TenantId);
