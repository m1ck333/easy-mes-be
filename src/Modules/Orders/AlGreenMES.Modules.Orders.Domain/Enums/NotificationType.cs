namespace AlGreenMES.Modules.Orders.Domain.Enums;

public enum NotificationType
{
    DeadlineWarning,
    DeadlineCritical,
    BlockRequest,
    BlockRequestApproved,
    ProcessCompleted,
    ProcessBlocked,
    OrderActivated
}
