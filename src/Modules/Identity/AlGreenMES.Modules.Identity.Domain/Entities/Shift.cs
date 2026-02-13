using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;

namespace AlGreenMES.Modules.Identity.Domain.Entities;

public class 
    Shift : AuditableEntity
{
    public string Name { get; private set; } = null!;
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public bool IsActive { get; private set; }

    private Shift()
    {
    }

    public static Shift Create(Guid tenantId, string name, TimeOnly startTime, TimeOnly endTime)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("SHIFT_NAME_REQUIRED", "Shift name is required.");

        var shift = new Shift
        {
            TenantId = tenantId,
            Name = name.Trim(),
            StartTime = startTime,
            EndTime = endTime,
            IsActive = true
        };

        return shift;
    }

    public void Update(string name, TimeOnly startTime, TimeOnly endTime, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("SHIFT_NAME_REQUIRED", "Shift name is required.");

        Name = name.Trim();
        StartTime = startTime;
        EndTime = endTime;
        IsActive = isActive;
    }
}
