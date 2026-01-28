using AlGreenMES.BuildingBlocks.Common.Interfaces;

namespace AlGreenMES.BuildingBlocks.Common.Entities;

/// <summary>
/// Base entity with tenant isolation and audit tracking support.
/// </summary>
public abstract class AuditableEntity : TenantEntity, IAuditableEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid tenantId) : base(tenantId)
    {
    }

    public void SetCreatedBy(string userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdatedBy(string userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
