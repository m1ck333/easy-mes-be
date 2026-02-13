using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.RemoveSpecialRequest;

public record RemoveSpecialRequestCommand(Guid OrderId, Guid OrderItemId, Guid SpecialRequestId) : IRequest<Unit>;
