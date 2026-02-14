using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.DeleteNotification;

public record DeleteNotificationCommand(Guid Id) : IRequest<Unit>;
