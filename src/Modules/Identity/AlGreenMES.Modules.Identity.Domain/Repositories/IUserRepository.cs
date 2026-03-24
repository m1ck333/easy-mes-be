using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Identity.Domain.Entities;

namespace AlGreenMES.Modules.Identity.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithProcessesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailWithProcessesAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, Guid tenantId, CancellationToken cancellationToken = default);
    Task<PagedResult<User>> GetPagedAsync(Guid tenantId, UserRole? role, bool? isActive, string? search, DateTime? createdFrom, DateTime? createdTo, string? sortBy, bool isDescending, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByProcessIdAsync(Guid processId, Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetDepartmentUsersWithProcessesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    void Delete(User user);
}
