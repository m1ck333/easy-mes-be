namespace AlGreenMES.Modules.Identity.Application.Services;

public record TenantLookupResult(Guid Id, string Code, bool IsActive);

public interface ITenantLookupService
{
    Task<TenantLookupResult?> GetTenantByCodeAsync(string code, CancellationToken cancellationToken = default);
}
