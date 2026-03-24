using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record BlockRequestDto(
    Guid Id,
    Guid? OrderItemProcessId,
    Guid? OrderItemSubProcessId,
    Guid RequestedByUserId,
    string? RequestNote,
    RequestStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid? HandledByUserId,
    DateTime? HandledAt,
    string? BlockReason,
    string? RejectionNote,
    Guid? OrderId,
    string? OrderNumber,
    ProcessStatus? CurrentProcessStatus,
    Guid? ProcessId,
    string? ProcessName);
