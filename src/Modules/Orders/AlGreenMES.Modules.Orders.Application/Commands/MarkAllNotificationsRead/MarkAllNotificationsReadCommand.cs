using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.MarkAllNotificationsRead;

public record MarkAllNotificationsReadCommand(Guid UserId) : IRequest<Unit>;
