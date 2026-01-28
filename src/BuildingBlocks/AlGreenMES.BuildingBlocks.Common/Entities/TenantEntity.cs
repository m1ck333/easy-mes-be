namespace AlGreenMES.BuildingBlocks.Common.Entities;

/// <summary>
/// Base entity with tenant isolation support.
/// </summary>
public abstract class TenantEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public Guid TenantId { get; protected set; }

    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
