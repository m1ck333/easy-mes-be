using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Repositories;

public class OrderItemSubProcessRepository : IOrderItemSubProcessRepository
{
    private readonly OrdersDbContext _dbContext;

    public OrderItemSubProcessRepository(OrdersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderItemSubProcess?> GetByIdWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrderItemSubProcesses
            .Include(sp => sp.Logs)
            .Include(sp => sp.OrderItemProcess)
                .ThenInclude(p => p.SubProcesses)
                    .ThenInclude(s => s.Logs)
            .Include(sp => sp.OrderItemProcess)
                .ThenInclude(p => p.OrderItem)
                    .ThenInclude(oi => oi.Order)
            .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<OrderItemSubProcessLog>> GetActiveLogsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<OrderItemSubProcessLog>()
            .Include(l => l.OrderItemSubProcess)
            .Where(l => l.UserId == userId && l.EndTime == null)
            .ToListAsync(cancellationToken);
    }
}
