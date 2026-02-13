using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Repositories;

public class ChangeRequestRepository : IChangeRequestRepository
{
    private readonly OrdersDbContext _dbContext;

    public ChangeRequestRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ChangeRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChangeRequests
            .FirstOrDefaultAsync(cr => cr.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ChangeRequest>> GetByTenantIdAsync(Guid tenantId, RequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ChangeRequests
            .Where(cr => cr.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(cr => cr.Status == status.Value);

        return await query
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChangeRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChangeRequests
            .Where(cr => cr.RequestedByUserId == userId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ChangeRequest request, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChangeRequests.AddAsync(request, cancellationToken);
    }
}
