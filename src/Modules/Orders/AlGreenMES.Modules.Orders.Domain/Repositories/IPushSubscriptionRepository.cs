using AlGreenMES.Modules.Orders.Domain.Entities;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IPushSubscriptionRepository
{
    Task<PushSubscription?> FindByEndpointActiveAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PushSubscription>> GetByUserIdActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PushSubscription>> GetByUserIdsActiveAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PushSubscription>> GetByTenantIdActiveAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserIdActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(PushSubscription subscription, CancellationToken cancellationToken = default);
}
