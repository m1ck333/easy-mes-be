using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.StartSubProcess;

public record StartSubProcessCommand(Guid OrderItemSubProcessId, Guid UserId) : IRequest<OrderItemSubProcessDto>;
