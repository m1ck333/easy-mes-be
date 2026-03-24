using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RestartProcess;

public record RestartProcessCommand(Guid OrderItemProcessId, bool ResetTime) : IRequest<Unit>;
