namespace AlGreenMES.Modules.Production.Domain.Entities;

public class ProductCategoryDependency
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid ProductCategoryId { get; private set; }
    public Guid ProcessId { get; private set; }
    public Guid DependsOnProcessId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ProductCategory ProductCategory { get; private set; } = null!;
    public Process Process { get; private set; } = null!;
    public Process DependsOnProcess { get; private set; } = null!;

    private ProductCategoryDependency()
    {
    }

    internal ProductCategoryDependency(Guid tenantId, Guid categoryId, Guid processId, Guid dependsOnProcessId)
    {
        TenantId = tenantId;
        ProductCategoryId = categoryId;
        ProcessId = processId;
        DependsOnProcessId = dependsOnProcessId;
    }
}
