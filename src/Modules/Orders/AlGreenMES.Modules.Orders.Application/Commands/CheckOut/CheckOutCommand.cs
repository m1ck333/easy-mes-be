using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CheckOut;

public record CheckOutCommand(Guid UserId) : IRequest<WorkSessionDto>;
