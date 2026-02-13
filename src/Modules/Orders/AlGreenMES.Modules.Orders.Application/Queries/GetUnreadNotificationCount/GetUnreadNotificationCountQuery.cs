using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetUnreadNotificationCount;

public record GetUnreadNotificationCountQuery(Guid UserId) : IRequest<int>;
