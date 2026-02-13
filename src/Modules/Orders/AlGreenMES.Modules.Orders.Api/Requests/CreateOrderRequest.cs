using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Api.Requests;

public record CreateOrderRequest(
    Guid TenantId,
    string OrderNumber,
    DateTime DeliveryDate,
    int Priority,
    OrderType OrderType,
    string? Notes);
