using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.CompleteProcess;

public record CompleteProcessCommand(Guid OrderItemProcessId) : IRequest<Unit>;
