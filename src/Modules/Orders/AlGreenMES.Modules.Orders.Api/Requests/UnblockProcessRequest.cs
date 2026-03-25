namespace AlGreenMES.Modules.Orders.Api.Requests;

public record UnblockProcessRequest(Guid UserId, bool ResetTime = false);
