using AlGreenMES.BuildingBlocks.Common.Entities;

namespace AlGreenMES.Modules.Orders.Domain.Entities;

public class PushSubscription : TenantEntity
{
    public Guid UserId { get; private set; }
    public string Endpoint { get; private set; } = null!;
    public string P256dhKey { get; private set; } = null!;
    public string AuthKey { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PushSubscription()
    {
    }

    public static PushSubscription Create(Guid tenantId, Guid userId, string endpoint, string p256dhKey, string authKey)
    {
        return new PushSubscription
        {
            TenantId = tenantId,
            UserId = userId,
            Endpoint = endpoint,
            P256dhKey = p256dhKey,
            AuthKey = authKey,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateKeys(string p256dhKey, string authKey)
    {
        P256dhKey = p256dhKey;
        AuthKey = authKey;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
