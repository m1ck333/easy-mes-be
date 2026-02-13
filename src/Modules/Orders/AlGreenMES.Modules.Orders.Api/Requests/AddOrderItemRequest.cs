namespace AlGreenMES.Modules.Orders.Api.Requests;

public record AddOrderItemRequest(
    Guid ProductCategoryId,
    string ProductName,
    int Quantity,
    string? Notes);
