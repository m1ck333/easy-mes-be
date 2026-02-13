using AlGreenMES.Modules.Orders.Domain.Entities;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IOrderItemSubProcessRepository
{
    Task<OrderItemSubProcess?> GetByIdWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
