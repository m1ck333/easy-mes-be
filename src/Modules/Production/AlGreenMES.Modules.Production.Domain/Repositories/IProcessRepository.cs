using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Domain.Entities;

namespace AlGreenMES.Modules.Production.Domain.Repositories;

public interface IProcessRepository
{
    Task<Process?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Process?> GetByIdWithSubProcessesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Process>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Process process, CancellationToken cancellationToken = default);
    void Remove(Process process);
    Task<bool> ExistsByCodeAsync(string code, Guid tenantId, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<PagedResult<Process>> GetPagedAsync(Guid tenantId, bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, string? sortBy, bool isDescending, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Process>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
