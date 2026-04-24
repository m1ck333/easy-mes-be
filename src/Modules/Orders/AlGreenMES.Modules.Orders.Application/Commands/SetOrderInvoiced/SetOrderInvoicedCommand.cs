using AlGreenMES.Modules.Orders.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.SetOrderInvoiced;

public record SetOrderInvoicedCommand(Guid OrderId, bool IsInvoiced) : IRequest<OrderDto>;
