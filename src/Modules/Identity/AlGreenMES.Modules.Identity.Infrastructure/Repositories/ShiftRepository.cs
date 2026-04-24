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

    public async Task<PagedResult<Shift>> GetPagedAsync(Guid tenantId, bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, string? sortBy, bool isDescending, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Shifts.Where(s => s.TenantId == tenantId);

        if (isActive.HasValue)
            query = query.Where(s => s.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(s));
        }

        if (createdFrom.HasValue)
        {
            var from = DateTime.SpecifyKind(createdFrom.Value.Date, DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt >= from);
        }
        if (createdTo.HasValue)
        {
            var to = DateTime.SpecifyKind(createdTo.Value.Date.AddDays(1), DateTimeKind.Utc);
            query = query.Where(x => x.CreatedAt < to);
        }

        IOrderedQueryable<Shift> sorted;
        switch (sortBy?.ToLowerInvariant())
        {
            case "starttime":
                sorted = isDescending ? query.OrderByDescending(s => s.StartTime) : query.OrderBy(s => s.StartTime);
                break;
            case "endtime":
                sorted = isDescending ? query.OrderByDescending(s => s.EndTime) : query.OrderBy(s => s.EndTime);
                break;
            case "createdat":
                sorted = isDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt);
                break;
            default: // name asc
                sorted = isDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                break;
        }

        return await sorted.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
