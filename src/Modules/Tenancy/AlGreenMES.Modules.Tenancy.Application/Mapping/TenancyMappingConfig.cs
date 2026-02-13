using AlGreenMES.Modules.Tenancy.Application.DTOs;
using AlGreenMES.Modules.Tenancy.Domain.Entities;
using Mapster;

namespace AlGreenMES.Modules.Tenancy.Application.Mapping;

public static class TenancyMappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tenant, TenantDto>();
        config.NewConfig<TenantSettings, TenantSettingsDto>();
    }
}
