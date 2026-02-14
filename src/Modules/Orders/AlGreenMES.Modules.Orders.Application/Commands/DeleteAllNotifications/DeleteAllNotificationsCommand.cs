using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.DeleteAllNotifications;

public record DeleteAllNotificationsCommand(Guid UserId) : IRequest<Unit>;
