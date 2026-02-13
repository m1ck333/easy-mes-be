using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RemoveOrderItem;

public record RemoveOrderItemCommand(Guid OrderId, Guid ItemId) : IRequest<Unit>;
