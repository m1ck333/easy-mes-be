using AlGreenMES.Modules.Orders.Domain.Entities;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IOrderAttachmentRepository
{
    Task<IReadOnlyList<OrderAttachment>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<OrderAttachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetCountByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task AddAsync(OrderAttachment attachment, CancellationToken cancellationToken = default);
    void Remove(OrderAttachment attachment);
    void RemoveRange(IEnumerable<OrderAttachment> attachments);
}
