namespace AlGreenMES.Modules.Tenancy.Api.Requests;

public record UpdateTenantSettingsRequest(
    int DefaultWarningDays,
    int DefaultCriticalDays,
    string WarningColor,
    string CriticalColor);
