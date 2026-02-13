using AlGreenMES.Modules.Orders.Domain.Entities;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IOrderItemProcessRepository
{
    Task<OrderItemProcess?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderItemProcess?> GetByIdWithSubProcessesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderItemProcess?> GetByIdWithOrderDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderItemProcess?> GetByIdWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderItemProcess>> GetByOrderItemIdAsync(Guid orderItemId, CancellationToken cancellationToken = default);
}
