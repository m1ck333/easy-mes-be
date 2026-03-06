using AlGreenMES.BuildingBlocks.Common.Entities;
using AlGreenMES.BuildingBlocks.Common.Exceptions;

namespace AlGreenMES.Modules.Production.Domain.Entities;

public class SpecialRequestType : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public List<Guid> AddsProcesses { get; private set; } = new();
    public List<Guid> RemovesProcesses { get; private set; } = new();
    public List<Guid> OnlyProcesses { get; private set; } = new();
    public bool IgnoresDependencies { get; private set; }
    public bool IsActive { get; private set; } = true;

    private SpecialRequestType()
    {
    }

    public static SpecialRequestType Create(Guid tenantId, string code, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("INVALID_CODE", "Special request type code is required.");
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "Special request type name is required.");

        return new SpecialRequestType
        {
            TenantId = tenantId,
            Code = code.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Description = description?.Trim()
        };
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("INVALID_NAME", "Special request type name is required.");

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void SetAddsProcesses(params Guid[] processIds)
    {
        AddsProcesses = processIds.ToList();
    }

    public void SetRemovesProcesses(params Guid[] processIds)
    {
        RemovesProcesses = processIds.ToList();
    }

    public void SetOnlyProcesses(params Guid[] processIds)
    {
        OnlyProcesses = processIds.ToList();
        IgnoresDependencies = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
