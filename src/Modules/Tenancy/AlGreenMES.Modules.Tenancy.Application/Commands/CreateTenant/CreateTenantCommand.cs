using AlGreenMES.Modules.Tenancy.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Commands.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string Code,
    int? DefaultWarningDays = null,
    int? DefaultCriticalDays = null,
    string? WarningColor = null,
    string? CriticalColor = null) : IRequest<TenantDto>;
