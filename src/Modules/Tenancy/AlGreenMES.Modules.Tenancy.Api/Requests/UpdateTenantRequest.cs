namespace AlGreenMES.Modules.Tenancy.Api.Requests;

public record UpdateTenantRequest(
    string Name,
    bool IsActive,
    int? DefaultWarningDays = null,
    int? DefaultCriticalDays = null,
    string? WarningColor = null,
    string? CriticalColor = null);
