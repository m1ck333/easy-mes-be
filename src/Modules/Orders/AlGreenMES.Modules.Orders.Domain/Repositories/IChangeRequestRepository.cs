using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IChangeRequestRepository
{
    Task<ChangeRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChangeRequest>> GetByTenantIdAsync(Guid tenantId, RequestStatus? status = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ChangeRequest>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(ChangeRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<ChangeRequest>> GetPagedAsync(Guid tenantId, RequestStatus? status, ChangeRequestType? requestType, string? search, DateTime? createdFrom, DateTime? createdTo, string? sortBy, bool isDescending, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<ChangeRequest>> GetPagedByUserAsync(Guid tenantId, Guid userId, RequestStatus? status, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
}
