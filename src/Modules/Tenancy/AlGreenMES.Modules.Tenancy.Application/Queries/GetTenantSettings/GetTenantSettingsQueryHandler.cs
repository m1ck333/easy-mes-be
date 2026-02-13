using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantSettings;

public class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, TenantSettingsDto?>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantSettingsQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantSettingsDto?> Handle(GetTenantSettingsQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant?.Settings is null)
            return null;

        return new TenantSettingsDto(
            tenant.Settings.Id,
            tenant.Settings.TenantId,
            tenant.Settings.DefaultWarningDays,
            tenant.Settings.DefaultCriticalDays,
            tenant.Settings.WarningColor,
            tenant.Settings.CriticalColor);
    }
}
