using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record ChangeRequestDto(
    Guid Id,
    Guid OrderId,
    Guid RequestedByUserId,
    ChangeRequestType RequestType,
    string Description,
    RequestStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    Guid? HandledByUserId,
    DateTime? HandledAt,
    string? ResponseNote);
