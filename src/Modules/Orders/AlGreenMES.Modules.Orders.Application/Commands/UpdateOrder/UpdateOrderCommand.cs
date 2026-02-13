using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.UpdateOrder;

public record UpdateOrderCommand(
    Guid Id,
    string? Notes,
    int? CustomWarningDays,
    int? CustomCriticalDays) : IRequest<OrderDto>;
