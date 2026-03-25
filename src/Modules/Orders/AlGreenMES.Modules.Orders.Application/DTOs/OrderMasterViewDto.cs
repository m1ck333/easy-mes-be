using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record OrderMasterViewDto(
    Guid Id,
    string OrderNumber,
    string OrderType,
    string Status,
    DateTime DeliveryDate,
    int Priority,
    int? CustomWarningDays,
    int? CustomCriticalDays,
    int CompletedProcesses,
    int TotalProcesses,
    Dictionary<string, string> ProcessStatuses,
    Dictionary<string, int> ProcessDurations,
    Dictionary<string, bool> ProcessPaused,
    /// <summary>Map of processId → list of processIds it depends on</summary>
    Dictionary<string, List<string>> ProcessDependencies,
    int AttachmentCount,
    DateTime CreatedAt);
