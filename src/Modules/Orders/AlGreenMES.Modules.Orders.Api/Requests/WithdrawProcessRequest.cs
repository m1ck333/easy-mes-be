namespace AlGreenMES.Modules.Orders.Api.Requests;

public record WithdrawProcessRequest(Guid UserId, string Reason);
