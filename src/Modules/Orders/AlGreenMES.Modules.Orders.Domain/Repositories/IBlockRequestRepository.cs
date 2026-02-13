using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Domain.Repositories;

public interface IBlockRequestRepository
{
    Task<BlockRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlockRequest>> GetByTenantIdAsync(Guid tenantId, RequestStatus? status = null, CancellationToken cancellationToken = default);
    Task AddAsync(BlockRequest request, CancellationToken cancellationToken = default);
    Task<PagedResult<BlockRequest>> GetPagedAsync(Guid tenantId, RequestStatus? status, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
}
