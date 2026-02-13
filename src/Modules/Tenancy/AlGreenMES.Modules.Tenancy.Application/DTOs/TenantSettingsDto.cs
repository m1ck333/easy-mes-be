namespace AlGreenMES.Modules.Tenancy.Application.DTOs;

public record TenantSettingsDto(
    Guid Id,
    Guid TenantId,
    int DefaultWarningDays,
    int DefaultCriticalDays,
    string WarningColor,
    string CriticalColor);
