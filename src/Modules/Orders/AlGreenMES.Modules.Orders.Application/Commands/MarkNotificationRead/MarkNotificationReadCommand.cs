using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid Id) : IRequest<Unit>;
