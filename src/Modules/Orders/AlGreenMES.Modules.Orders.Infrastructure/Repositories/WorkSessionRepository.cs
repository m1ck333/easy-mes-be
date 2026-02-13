using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Repositories;

public class WorkSessionRepository : IWorkSessionRepository
{
    private readonly OrdersDbContext _dbContext;

    public WorkSessionRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkSessions
            .FirstOrDefaultAsync(ws => ws.Id == id, cancellationToken);
    }

    public async Task<WorkSession?> GetActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkSessions
            .FirstOrDefaultAsync(ws => ws.UserId == userId && ws.CheckOutTime == null, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkSession>> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkSessions
            .Where(ws => ws.UserId == userId && ws.Date == date)
            .OrderBy(ws => ws.CheckInTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WorkSession>> GetByTenantAndDateAsync(Guid tenantId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkSessions
            .Where(ws => ws.TenantId == tenantId && ws.Date == date)
            .OrderBy(ws => ws.CheckInTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(WorkSession session, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkSessions.AddAsync(session, cancellationToken);
    }

    public async Task<PagedResult<WorkSession>> GetPagedAsync(Guid tenantId, DateOnly date, Guid? userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.WorkSessions.Where(ws => ws.TenantId == tenantId && ws.Date == date);

        if (userId.HasValue)
            query = query.Where(ws => ws.UserId == userId.Value);

        query = query.OrderBy(ws => ws.CheckInTime);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
