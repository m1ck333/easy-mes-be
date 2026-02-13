using AlGreenMES.Modules.Identity.Application.DTOs;
using AlGreenMES.Modules.Identity.Domain.Entities;
using Mapster;

namespace AlGreenMES.Modules.Identity.Application.Mapping;

public static class IdentityMappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        config.NewConfig<Shift, ShiftDto>();
    }
}
