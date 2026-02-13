using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ActivateOrder;

public record ActivateOrderCommand(Guid Id) : IRequest<Unit>;
