using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using AlGreenMES.Modules.Production.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlgreenMES.API.Services;

public class ReferenceCheckService : IReferenceCheckService
{
    private readonly OrdersDbContext _ordersDb;

    public ReferenceCheckService(OrdersDbContext ordersDb)
    {
        _ordersDb = ordersDb;
    }

    public async Task<int> CountCategoryOrderReferencesAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _ordersDb.Set<AlGreenMES.Modules.Orders.Domain.Entities.OrderItem>()
            .Where(oi => oi.ProductCategoryId == categoryId)
            .Select(oi => oi.OrderId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountProcessOrderReferencesAsync(Guid processId, CancellationToken cancellationToken = default)
    {
        return await _ordersDb.Set<AlGreenMES.Modules.Orders.Domain.Entities.OrderItemProcess>()
            .Where(oip => oip.ProcessId == processId)
            .Select(oip => oip.OrderItemId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task NullifyCategoryReferencesAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        await _ordersDb.Database.ExecuteSqlRawAsync(
            "UPDATE orders.order_items SET product_category_id = NULL WHERE product_category_id = {0}", new object[] { categoryId });
    }

    public async Task NullifyProcessReferencesAsync(Guid processId, CancellationToken cancellationToken = default)
    {
        await _ordersDb.Database.ExecuteSqlRawAsync(
            "DELETE FROM orders.order_item_sub_processes WHERE order_item_process_id IN (SELECT id FROM orders.order_item_processes WHERE process_id = {0})", new object[] { processId });
        await _ordersDb.Database.ExecuteSqlRawAsync(
            "DELETE FROM orders.order_item_processes WHERE process_id = {0}", new object[] { processId });
    }
}
