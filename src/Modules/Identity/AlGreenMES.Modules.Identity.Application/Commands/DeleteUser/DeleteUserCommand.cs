using MediatR;

namespace AlGreenMES.Modules.Identity.Application.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<Unit>;
