using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.PauseOrder;

public record PauseOrderCommand(Guid Id) : IRequest<Unit>;
