using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class Notification : TenantEntity
{
    public Guid UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? ReferenceType { get; private set; }
    public Guid? ReferenceId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Notification()
    {
    }

    public static Notification Create(Guid tenantId, Guid userId, NotificationType type,
        string title, string message, string? referenceType = null, Guid? referenceId = null)
    {
        return new Notification
        {
            TenantId = tenantId,
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            IsRead = false
        };
    }

    public void MarkRead()
    {
        IsRead = true;
    }

    public void MarkUnread()
    {
        IsRead = false;
    }
}
