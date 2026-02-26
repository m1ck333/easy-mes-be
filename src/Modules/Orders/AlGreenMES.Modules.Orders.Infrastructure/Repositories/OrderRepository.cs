using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _dbContext;

    public OrderRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByIdWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Processes)
                    .ThenInclude(p => p.SubProcesses)
            .Include(o => o.Items)
                .ThenInclude(i => i.SpecialRequests)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetByTenantIdAsync(Guid tenantId, OrderStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        return await query
            .OrderBy(o => o.Priority)
            .ThenBy(o => o.DeliveryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public void AddItem(OrderItem item)
    {
        _dbContext.OrderItems.Add(item);
    }

    public async Task<IReadOnlyList<Order>> GetActiveOrdersWithProcessesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Processes)
                    .ThenInclude(p => p.SubProcesses)
                        .ThenInclude(sp => sp.Logs)
            .Include(o => o.Items)
                .ThenInclude(i => i.SpecialRequests)
            .Where(o => o.TenantId == tenantId && o.Status == OrderStatus.Active)
            .OrderBy(o => o.Priority)
            .ThenBy(o => o.DeliveryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var normalized = orderNumber.Trim();
        return await _dbContext.Orders
            .AnyAsync(o => o.OrderNumber == normalized && o.TenantId == tenantId, cancellationToken);
    }

    public async Task<PagedResult<Order>> GetPagedAsync(Guid tenantId, OrderStatus? status, OrderType? orderType, DateTime? dateFrom, DateTime? dateTo, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (orderType.HasValue)
            query = query.Where(o => o.OrderType == orderType.Value);

        if (dateFrom.HasValue)
            query = query.Where(o => o.DeliveryDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(o => o.DeliveryDate <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o => o.OrderNumber.Contains(search));

        query = query.OrderBy(o => o.Priority).ThenBy(o => o.DeliveryDate);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<PagedResult<Order>> GetPagedWithProcessesAsync(Guid tenantId, OrderStatus? status, OrderType? orderType, DateTime? dateFrom, DateTime? dateTo, string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Processes)
            .Include(o => o.Attachments)
            .Where(o => o.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (orderType.HasValue)
            query = query.Where(o => o.OrderType == orderType.Value);

        if (dateFrom.HasValue)
            query = query.Where(o => o.DeliveryDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(o => o.DeliveryDate <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o => o.OrderNumber.Contains(search));

        query = query.OrderBy(o => o.Priority).ThenBy(o => o.DeliveryDate);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
