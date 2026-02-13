using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Production.Infrastructure.Repositories;

public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly ProductionDbContext _dbContext;

    public ProductCategoryRepository(ProductionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductCategories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<ProductCategory?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductCategories
            .Include(c => c.Processes)
                .ThenInclude(p => p.Process)
            .Include(c => c.Dependencies)
                .ThenInclude(d => d.Process)
            .Include(c => c.Dependencies)
                .ThenInclude(d => d.DependsOnProcess)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProductCategory>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ProductCategories
            .Where(c => c.TenantId == tenantId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ProductCategory category, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProductCategories.AddAsync(category, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        return await _dbContext.ProductCategories
            .AnyAsync(c => c.Name == normalizedName && c.TenantId == tenantId, cancellationToken);
    }
}
