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
    int AttachmentCount);
