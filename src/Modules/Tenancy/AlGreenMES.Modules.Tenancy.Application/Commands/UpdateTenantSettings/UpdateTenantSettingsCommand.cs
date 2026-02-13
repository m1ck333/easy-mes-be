using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Commands.UpdateTenantSettings;

public record UpdateTenantSettingsCommand(
    Guid TenantId,
    int DefaultWarningDays,
    int DefaultCriticalDays,
    string WarningColor,
    string CriticalColor) : IRequest<TenantSettingsDto>;
