using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CancelOrder;

public record CancelOrderCommand(Guid Id) : IRequest<Unit>;
