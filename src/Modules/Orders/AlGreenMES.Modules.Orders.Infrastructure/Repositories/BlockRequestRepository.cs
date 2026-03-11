using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Repositories;

public class BlockRequestRepository : IBlockRequestRepository
{
    private readonly OrdersDbContext _dbContext;

    public BlockRequestRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BlockRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BlockRequests
            .FirstOrDefaultAsync(br => br.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BlockRequest>> GetByTenantIdAsync(Guid tenantId, RequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.BlockRequests
            .Where(br => br.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(br => br.Status == status.Value);

        return await query
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BlockRequest request, CancellationToken cancellationToken = default)
    {
        await _dbContext.BlockRequests.AddAsync(request, cancellationToken);
    }

    public async Task<PagedResult<BlockRequest>> GetPagedAsync(Guid tenantId, RequestStatus? status, string? search, DateTime? createdFrom, DateTime? createdTo, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.BlockRequests
            .Include(br => br.OrderItemProcess)
                .ThenInclude(p => p!.OrderItem)
                    .ThenInclude(i => i.Order)
            .Where(br => br.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(br => br.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(br => br.RequestNote != null && br.RequestNote.Contains(search));

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

        query = query.OrderByDescending(br => br.CreatedAt);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
