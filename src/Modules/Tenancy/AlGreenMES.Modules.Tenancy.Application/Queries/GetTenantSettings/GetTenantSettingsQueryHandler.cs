using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Tenancy.Application.Queries.GetTenantSettings;

public class GetTenantSettingsQueryHandler : IRequestHandler<GetTenantSettingsQuery, TenantSettingsDto>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantSettingsQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantSettingsDto> Handle(GetTenantSettingsQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.TenantId, cancellationToken)
            ?? throw new NotFoundException("Tenant", request.TenantId);

        if (tenant.Settings is null)
            throw new NotFoundException("TenantSettings", request.TenantId);

        return tenant.Settings.Adapt<TenantSettingsDto>();
    }
}
