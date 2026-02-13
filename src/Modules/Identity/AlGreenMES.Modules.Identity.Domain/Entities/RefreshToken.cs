using AlGreenMES.BuildingBlocks.Common.Entities;

namespace AlGreenMES.Modules.Identity.Domain.Entities;

public class RefreshToken : TenantEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private RefreshToken()
    {
    }

    public static RefreshToken Create(Guid tenantId, Guid userId, string token, DateTime expiresAt)
    {
        return new RefreshToken
        {
            TenantId = tenantId,
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsValid() => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public void Revoke()
    {
        IsRevoked = true;
    }
}
