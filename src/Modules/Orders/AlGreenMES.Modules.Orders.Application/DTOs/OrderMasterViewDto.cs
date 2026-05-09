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
    /// <summary>
    /// Map of processId → true if at least ONE item has this process Pending and
    /// all of that item's dependencies for it are Completed or Withdrawn. Computed
    /// per-item because the aggregated ProcessStatuses can't tell that one item is
    /// ready when sibling items are still mid-pipeline.
    /// </summary>
    Dictionary<string, bool> ProcessReady,
    /// <summary>Map of processId → list of processIds it depends on</summary>
    Dictionary<string, List<string>> ProcessDependencies,
    int AttachmentCount,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    bool IsInvoiced);
