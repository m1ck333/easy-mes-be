using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.MarkNotificationUnread;

public record MarkNotificationUnreadCommand(Guid Id) : IRequest<Unit>;
