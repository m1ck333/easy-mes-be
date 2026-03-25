using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record OrderItemSubProcessDto(
    Guid Id,
    Guid OrderItemProcessId,
    Guid SubProcessId,
    SubProcessStatus Status,
    int TotalDurationMinutes,
    bool IsWithdrawn,
    bool IsTimerRunning,
    DateTime? CurrentLogStartedAt);
