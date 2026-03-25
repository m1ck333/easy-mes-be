using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.UnblockProcess;

public record UnblockProcessCommand(Guid OrderItemProcessId, Guid UserId, bool ResetTime = false) : IRequest<Unit>;
