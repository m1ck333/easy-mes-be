using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ReopenOrder;

public record ReopenOrderCommand(Guid Id) : IRequest<Unit>;
