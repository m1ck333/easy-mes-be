using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.WithdrawProcess;

public record WithdrawProcessCommand(Guid OrderItemProcessId, Guid UserId, string Reason) : IRequest<Unit>;
