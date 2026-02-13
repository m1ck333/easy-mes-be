using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Production.Infrastructure.Repositories;

public class SpecialRequestTypeRepository : ISpecialRequestTypeRepository
{
    private readonly ProductionDbContext _dbContext;

    public SpecialRequestTypeRepository(ProductionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SpecialRequestType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SpecialRequestTypes
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialRequestType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SpecialRequestTypes
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SpecialRequestType specialRequestType, CancellationToken cancellationToken = default)
    {
        await _dbContext.SpecialRequestTypes.AddAsync(specialRequestType, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        return await _dbContext.SpecialRequestTypes
            .AnyAsync(s => s.Code == normalizedCode && s.TenantId == tenantId, cancellationToken);
    }
}
