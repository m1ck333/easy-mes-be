namespace AlGreenMES.Modules.Orders.Application.DTOs;

public record OrderItemDto(
    Guid Id,
    Guid OrderId,
    Guid ProductCategoryId,
    string ProductName,
    int Quantity,
    string? Notes,
    List<OrderItemProcessDto> Processes,
    List<OrderItemSpecialRequestDto> SpecialRequests);
