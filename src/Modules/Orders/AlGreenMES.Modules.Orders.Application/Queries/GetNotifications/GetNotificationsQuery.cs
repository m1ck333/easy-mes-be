using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetNotifications;

public record GetNotificationsQuery : PagedQuery<PagedResult<NotificationDto>>
{
    public Guid UserId { get; init; }
    public bool? IsRead { get; init; }
}
