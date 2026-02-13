using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Production.Domain.Entities;

public class ProductCategoryProcess
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public Guid ProductCategoryId { get; private set; }
    public Guid ProcessId { get; private set; }
    public ComplexityType? DefaultComplexity { get; private set; }
    public int SequenceOrder { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ProductCategory ProductCategory { get; private set; } = null!;
    public Process Process { get; private set; } = null!;

    private ProductCategoryProcess()
    {
    }

    internal ProductCategoryProcess(Guid tenantId, Guid categoryId, Guid processId, int sequenceOrder, ComplexityType? complexity)
    {
        TenantId = tenantId;
        ProductCategoryId = categoryId;
        ProcessId = processId;
        SequenceOrder = sequenceOrder;
        DefaultComplexity = complexity;
    }
}
