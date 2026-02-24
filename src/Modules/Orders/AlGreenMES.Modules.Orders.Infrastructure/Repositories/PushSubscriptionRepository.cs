using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Repositories;

public class PushSubscriptionRepository : IPushSubscriptionRepository
{
    private readonly OrdersDbContext _dbContext;

    public PushSubscriptionRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PushSubscription?> FindByEndpointActiveAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PushSubscriptions
            .FirstOrDefaultAsync(p => p.Endpoint == endpoint && p.IsActive, cancellationToken);
    }

    public async Task<IReadOnlyList<PushSubscription>> GetByUserIdActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PushSubscriptions
            .Where(p => p.UserId == userId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PushSubscription>> GetByUserIdsActiveAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        var ids = userIds.ToList();
        return await _dbContext.PushSubscriptions
            .Where(p => ids.Contains(p.UserId) && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PushSubscription>> GetByTenantIdActiveAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PushSubscriptions
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByUserIdActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PushSubscriptions
            .AnyAsync(p => p.UserId == userId && p.IsActive, cancellationToken);
    }

    public async Task AddAsync(PushSubscription subscription, CancellationToken cancellationToken = default)
    {
        await _dbContext.PushSubscriptions.AddAsync(subscription, cancellationToken);
    }
}
