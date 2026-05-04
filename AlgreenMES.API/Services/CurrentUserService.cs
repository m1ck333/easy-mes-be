using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AlGreenMES.BuildingBlocks.Common.Interfaces;

namespace AlgreenMES.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;

    public Guid GetCurrentTenantId()
    {
        var ctx = _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("No HTTP context — cannot resolve tenant.");

        var claim = ctx.User?.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var tenantId))
        {
            throw new UnauthorizedAccessException("Missing or invalid tenant_id claim.");
        }
        return tenantId;
    }

    public Guid GetCurrentUserId()
    {
        var ctx = _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("No HTTP context — cannot resolve user.");

        var claim = ctx.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? ctx.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(claim) || !Guid.TryParse(claim, out var userId))
        {
            throw new UnauthorizedAccessException("Missing or invalid user id claim.");
        }
        return userId;
    }
}
