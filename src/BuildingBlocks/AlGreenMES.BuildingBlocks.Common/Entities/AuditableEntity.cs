using AlGreenMES.BuildingBlocks.Common.Interfaces;

namespace AlGreenMES.BuildingBlocks.Common.Entities;

/// <summary>
/// Base entity with tenant isolation and audit tracking support.
/// </summary>
public abstract class AuditableEntity : TenantEntity, IAuditableEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid tenantId) : base(tenantId)
    {
    }

    public void SetCreated(Guid userId)
    {
        CreatedByUserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdated(Guid userId)
    {
        UpdatedByUserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
