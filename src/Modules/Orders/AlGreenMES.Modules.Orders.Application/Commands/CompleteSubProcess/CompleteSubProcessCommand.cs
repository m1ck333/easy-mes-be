using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CompleteSubProcess;

public record CompleteSubProcessCommand(Guid OrderItemSubProcessId, Guid UserId) : IRequest<OrderItemSubProcessDto>;
