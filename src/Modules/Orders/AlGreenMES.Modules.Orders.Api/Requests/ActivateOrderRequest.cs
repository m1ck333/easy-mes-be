namespace AlGreenMES.Modules.Orders.Api.Requests;

public record ActivateOrderRequest(List<Guid>? ResetProcessIds = null);
