using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs.Tablet;

public record TabletActiveWorkDto(
    Guid OrderItemProcessId,
    Guid OrderId,
    string OrderNumber,
    int Priority,
    DateTime DeliveryDate,
    string ProductName,
    int Quantity,
    ComplexityType? Complexity,
    ProcessStatus Status,
    DateTime? StartedAt,
    int TotalDurationMinutes,
    bool IsTimerRunning,
    DateTime? CurrentLogStartedAt,
    List<string> SpecialRequestNames,
    int CompletedProcessCount,
    int TotalProcessCount,
    List<TabletSubProcessDto> SubProcesses);

public record TabletSubProcessDto(
    Guid Id,
    Guid SubProcessId,
    SubProcessStatus Status,
    int TotalDurationMinutes,
    bool IsWithdrawn,
    bool IsTimerRunning);
