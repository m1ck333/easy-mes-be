using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Identity.Domain.Entities;

namespace AlGreenMES.Modules.Identity.Domain.Repositories;

public interface IShiftRepository
{
    Task<Shift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shift>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(Shift shift, CancellationToken cancellationToken = default);
    Task<PagedResult<Shift>> GetPagedAsync(Guid tenantId, bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, int page, int pageSize, CancellationToken cancellationToken = default);
}
