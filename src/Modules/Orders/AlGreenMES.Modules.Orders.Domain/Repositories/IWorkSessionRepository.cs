using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Domain.Entities;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IWorkSessionRepository
{
    Task<WorkSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkSession?> GetActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkSession>> GetByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkSession>> GetByTenantAndDateAsync(Guid tenantId, DateTime date, CancellationToken cancellationToken = default);
    Task AddAsync(WorkSession session, CancellationToken cancellationToken = default);
    Task<PagedResult<WorkSession>> GetPagedAsync(Guid tenantId, DateTime date, Guid? userId, int page, int pageSize, CancellationToken cancellationToken = default);
}
