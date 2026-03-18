using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class WorkSession : TenantEntity
{
    public Guid UserId { get; private set; }
    public DateTime CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }
    public int? DurationMinutes { get; private set; }
    public DateOnly Date { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public bool IsActive => !CheckOutTime.HasValue;

    private WorkSession()
    {
    }

    public static WorkSession CheckIn(Guid tenantId, Guid userId)
    {
        return new WorkSession
        {
            TenantId = tenantId,
            UserId = userId,
            CheckInTime = DateTime.UtcNow,
            Date = DateOnly.FromDateTime(DateTime.UtcNow)
        };
    }

    public void CheckOut()
    {
        if (CheckOutTime.HasValue)
            throw new DomainException("ALREADY_CHECKED_OUT", "Already checked out.");
        CheckOutTime = DateTime.UtcNow;
        DurationMinutes = (int)(CheckOutTime.Value - CheckInTime).TotalMinutes;
        UpdatedAt = DateTime.UtcNow;
    }
}
