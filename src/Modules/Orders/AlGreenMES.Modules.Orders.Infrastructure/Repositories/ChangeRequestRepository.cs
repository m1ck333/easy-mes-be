using AlGreenMES.BuildingBlocks.Common.Pagination;
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

    public async Task<PagedResult<ChangeRequest>> GetPagedAsync(Guid tenantId, RequestStatus? status, ChangeRequestType? requestType, string? search, DateTime? createdFrom, DateTime? createdTo, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ChangeRequests.Where(cr => cr.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(cr => cr.Status == status.Value);

        if (requestType.HasValue)
            query = query.Where(cr => cr.RequestType == requestType.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(cr => cr.Description.Contains(search));

        if (createdFrom.HasValue)
            query = query.Where(x => x.CreatedAt >= createdFrom.Value);
        if (createdTo.HasValue)
            query = query.Where(x => x.CreatedAt < createdTo.Value.AddDays(1));

        query = query.OrderByDescending(cr => cr.CreatedAt);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<PagedResult<ChangeRequest>> GetPagedByUserAsync(Guid tenantId, Guid userId, RequestStatus? status, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ChangeRequests.Where(cr => cr.TenantId == tenantId && cr.RequestedByUserId == userId);

        if (status.HasValue)
            query = query.Where(cr => cr.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(cr => cr.Description.Contains(search));

        query = query.OrderByDescending(cr => cr.CreatedAt);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
