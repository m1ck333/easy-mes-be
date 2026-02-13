using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.ResumeProcessWork;

public record ResumeProcessWorkCommand(Guid OrderItemProcessId, Guid UserId) : IRequest<Unit>;
