namespace AlGreenMES.Modules.Tenancy.Domain.Entities;

public class TenantSettings
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public int DefaultWarningDays { get; private set; }
    public int DefaultCriticalDays { get; private set; }
    public string WarningColor { get; private set; } = null!;
    public string CriticalColor { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    public Tenant Tenant { get; private set; } = null!;

    private TenantSettings()
    {
    }

    public static TenantSettings CreateDefault(Guid tenantId)
    {
        return new TenantSettings
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            DefaultWarningDays = 7,
            DefaultCriticalDays = 3,
            WarningColor = "#FFA500",
            CriticalColor = "#FF0000"
        };
    }

    public void Update(int defaultWarningDays, int defaultCriticalDays, string warningColor, string criticalColor)
    {
        DefaultWarningDays = defaultWarningDays;
        DefaultCriticalDays = defaultCriticalDays;
        WarningColor = warningColor;
        CriticalColor = criticalColor;
        UpdatedAt = DateTime.UtcNow;
    }
}
