using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ResumeOrder;

public record ResumeOrderCommand(Guid Id) : IRequest<Unit>;
