using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;

namespace AlGreenMES.Modules.Production.Domain.Entities;

public class SubProcess : AuditableEntity
{
    public Guid ProcessId { get; private set; }
    public string Name { get; private set; } = null!;
    public int SequenceOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Process Process { get; private set; } = null!;

    private SubProcess()
    {
    }

    internal static SubProcess Create(Guid tenantId, Guid processId, string name, int sequenceOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "Sub-process name is required.");

        return new SubProcess
        {
            TenantId = tenantId,
            ProcessId = processId,
            Name = name.Trim(),
            SequenceOrder = sequenceOrder
        };
    }

    public void Update(string name, int sequenceOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "Sub-process name is required.");

        Name = name.Trim();
        SequenceOrder = sequenceOrder;
    }

    public void Deactivate() => IsActive = false;
}
