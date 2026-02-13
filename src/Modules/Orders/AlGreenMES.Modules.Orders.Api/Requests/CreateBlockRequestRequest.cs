namespace AlGreenMES.Modules.Orders.Api.Requests;

public record CreateBlockRequestRequest(
    Guid TenantId,
    Guid? OrderItemProcessId,
    Guid? OrderItemSubProcessId,
    Guid RequestedByUserId,
    string? RequestNote);
