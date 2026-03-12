using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record OrderDetailDto(
    Guid Id,
    Guid TenantId,
    string OrderNumber,
    DateTime DeliveryDate,
    int Priority,
    OrderType OrderType,
    OrderStatus Status,
    string? Notes,
    int? CustomWarningDays,
    int? CustomCriticalDays,
    List<OrderItemDto> Items,
    List<OrderAttachmentDto> Attachments);
