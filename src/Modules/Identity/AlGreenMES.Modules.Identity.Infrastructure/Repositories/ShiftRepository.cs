using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Identity.Domain.Entities;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using AlGreenMES.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Identity.Infrastructure.Repositories;

public class ShiftRepository : IShiftRepository
{
    private readonly IdentityDbContext _dbContext;

    public ShiftRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shifts
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Shift>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shifts
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Shift shift, CancellationToken cancellationToken = default)
    {
        await _dbContext.Shifts.AddAsync(shift, cancellationToken);
    }

    public async Task<PagedResult<Shift>> GetPagedAsync(Guid tenantId, bool? isActive, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Shifts.Where(s => s.TenantId == tenantId);

        if (isActive.HasValue)
            query = query.Where(s => s.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search));

        query = query.OrderBy(s => s.Name);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
