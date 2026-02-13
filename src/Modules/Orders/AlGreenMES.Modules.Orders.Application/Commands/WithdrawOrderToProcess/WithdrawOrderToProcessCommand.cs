using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.WithdrawOrderToProcess;

public record WithdrawOrderToProcessCommand(Guid OrderId, Guid TargetProcessId, string Reason, Guid UserId) : IRequest<Unit>;
