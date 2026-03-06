namespace AlGreenMES.Modules.Tenancy.Api.Requests;

public record CreateTenantRequest(
    string Name,
    string Code,
    int? DefaultWarningDays = null,
    int? DefaultCriticalDays = null,
    string? WarningColor = null,
    string? CriticalColor = null);
