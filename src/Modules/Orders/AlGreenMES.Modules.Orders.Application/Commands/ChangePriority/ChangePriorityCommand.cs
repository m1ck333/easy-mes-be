using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ChangePriority;

public record ChangePriorityCommand(Guid OrderId, int Priority) : IRequest<OrderDto>;
