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
}
