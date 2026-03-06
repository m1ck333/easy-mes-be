using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Domain.Entities;

namespace AlGreenMES.Modules.Production.Domain.Repositories;

public interface ISpecialRequestTypeRepository
{
    Task<SpecialRequestType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpecialRequestType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(SpecialRequestType specialRequestType, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, Guid tenantId, CancellationToken cancellationToken = default);
    Task<PagedResult<SpecialRequestType>> GetPagedAsync(Guid tenantId, bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, int page, int pageSize, CancellationToken cancellationToken = default);
}
