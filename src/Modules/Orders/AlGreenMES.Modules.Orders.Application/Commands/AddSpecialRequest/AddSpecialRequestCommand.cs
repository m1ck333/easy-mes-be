using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.AddSpecialRequest;

public record AddSpecialRequestCommand(Guid OrderId, Guid OrderItemId, Guid SpecialRequestTypeId) : IRequest<Unit>;
