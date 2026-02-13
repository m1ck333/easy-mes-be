namespace AlGreenMES.Modules.Orders.Api.Requests;

public record WithdrawOrderToProcessRequest(Guid TargetProcessId, string Reason, Guid UserId);
