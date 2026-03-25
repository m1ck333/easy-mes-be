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
                        .ThenInclude(sp => sp.Logs)
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
        {
            var from = DateTime.SpecifyKind(dateFrom.Value.Date, DateTimeKind.Utc);
            query = query.Where(o => o.DeliveryDate >= from);
        }

        if (dateTo.HasValue)
        {
            var to = DateTime.SpecifyKind(dateTo.Value.Date.AddDays(1), DateTimeKind.Utc);
            query = query.Where(o => o.DeliveryDate < to);
        }

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o => o.OrderNumber.Contains(search));

        query = query
            .OrderBy(o => o.Status == OrderStatus.Completed ? 1 : 0)
            .ThenBy(o => o.Status == OrderStatus.Cancelled ? 1 : 0)
            .ThenBy(o => o.Priority)
            .ThenBy(o => o.DeliveryDate);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<PagedResult<Order>> GetPagedWithProcessesAsync(Guid tenantId, OrderStatus? status, OrderType? orderType, DateTime? dateFrom, DateTime? dateTo, string? search, string? sortBy, bool isDescending, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Processes)
                    .ThenInclude(p => p.SubProcesses)
                        .ThenInclude(sp => sp.Logs)
            .Include(o => o.Attachments)
            .Where(o => o.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (orderType.HasValue)
            query = query.Where(o => o.OrderType == orderType.Value);

        if (dateFrom.HasValue)
        {
            var from = DateTime.SpecifyKind(dateFrom.Value.Date, DateTimeKind.Utc);
            query = query.Where(o => o.DeliveryDate >= from);
        }

        if (dateTo.HasValue)
        {
            var to = DateTime.SpecifyKind(dateTo.Value.Date.AddDays(1), DateTimeKind.Utc);
            query = query.Where(o => o.DeliveryDate < to);
        }

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o => o.OrderNumber.Contains(search));

        // Dynamic sorting
        IOrderedQueryable<Order> sorted;
        switch (sortBy?.ToLowerInvariant())
        {
            case "ordernumber":
                sorted = isDescending ? query.OrderByDescending(o => o.OrderNumber) : query.OrderBy(o => o.OrderNumber);
                break;
            case "ordertype":
                sorted = isDescending ? query.OrderByDescending(o => o.OrderType) : query.OrderBy(o => o.OrderType);
                break;
            case "status":
                // Active=0, Paused=1, Draft=2, Cancelled=3, Completed=4
                if (isDescending)
                    sorted = query.OrderByDescending(o =>
                        o.Status == OrderStatus.Active ? 0 :
                        o.Status == OrderStatus.Paused ? 1 :
                        o.Status == OrderStatus.Cancelled ? 3 :
                        o.Status == OrderStatus.Completed ? 4 : 2);
                else
                    sorted = query.OrderBy(o =>
                        o.Status == OrderStatus.Active ? 0 :
                        o.Status == OrderStatus.Paused ? 1 :
                        o.Status == OrderStatus.Cancelled ? 3 :
                        o.Status == OrderStatus.Completed ? 4 : 2);
                break;
            case "createdat":
                sorted = isDescending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt);
                break;
            case "deliverydate":
                sorted = isDescending ? query.OrderByDescending(o => o.DeliveryDate) : query.OrderBy(o => o.DeliveryDate);
                break;
            default: // priority (default)
                sorted = query
                    .OrderBy(o => o.Status == OrderStatus.Completed ? 1 : 0)
                    .ThenBy(o => o.Status == OrderStatus.Cancelled ? 1 : 0);
                sorted = isDescending ? sorted.ThenByDescending(o => o.Priority) : sorted.ThenBy(o => o.Priority);
                break;
        }

        return await sorted.ToPagedResultAsync(page, pageSize, cancellationToken);
    }
}
