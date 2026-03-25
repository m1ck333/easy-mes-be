using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ActivateOrder;

public record ActivateOrderCommand(Guid Id, List<Guid>? ResetProcessIds = null) : IRequest<Unit>;
