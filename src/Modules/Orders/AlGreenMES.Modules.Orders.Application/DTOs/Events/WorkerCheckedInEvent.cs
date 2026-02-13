namespace AlGreenMES.Modules.Orders.Application.DTOs.Events;

public record WorkerCheckedInEvent(
    Guid UserId,
    Guid ProcessId,
    Guid SessionId,
    Guid TenantId);
