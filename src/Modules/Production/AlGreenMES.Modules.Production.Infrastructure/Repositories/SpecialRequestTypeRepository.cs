using AlGreenMES.BuildingBlocks.Common.Pagination;
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

    public async Task<PagedResult<SpecialRequestType>> GetPagedAsync(Guid tenantId, bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, string? sortBy, bool isDescending, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SpecialRequestTypes.Where(srt => srt.TenantId == tenantId);

        if (isActive.HasValue)
            query = query.Where(srt => srt.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(srt => srt.Name.ToLower().Contains(s) || srt.Code.ToLower().Contains(s));
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

        IOrderedQueryable<SpecialRequestType> sorted;
        switch (sortBy?.ToLowerInvariant())
        {
            case "name":
                sorted = isDescending ? query.OrderByDescending(srt => srt.Name) : query.OrderBy(srt => srt.Name);
                break;
            case "createdat":
                sorted = isDescending ? query.OrderByDescending(srt => srt.CreatedAt) : query.OrderBy(srt => srt.CreatedAt);
                break;
            default: // code asc
                sorted = isDescending ? query.OrderByDescending(srt => srt.Code) : query.OrderBy(srt => srt.Code);
                break;
        }

        return await sorted.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
