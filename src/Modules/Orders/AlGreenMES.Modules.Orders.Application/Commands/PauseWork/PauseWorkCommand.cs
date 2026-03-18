using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.PauseWork;

public record PauseWorkCommand(Guid UserId) : IRequest<Unit>;
