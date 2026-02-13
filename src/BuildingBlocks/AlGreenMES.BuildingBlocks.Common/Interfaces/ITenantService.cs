namespace AlGreenMES.BuildingBlocks.Common.Interfaces;

/// <summary>
/// Service for resolving the current tenant from the request context (e.g., JWT claims).
/// </summary>
public interface ITenantService
{
    Guid GetCurrentTenantId();
}
