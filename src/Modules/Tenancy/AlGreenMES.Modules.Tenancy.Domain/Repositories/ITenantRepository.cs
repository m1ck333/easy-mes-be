using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Tenancy.Domain.Entities;

namespace AlGreenMES.Modules.Tenancy.Domain.Repositories;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<PagedResult<Tenant>> GetPagedAsync(bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, int page, int pageSize, CancellationToken cancellationToken = default);
}
