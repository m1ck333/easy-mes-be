using AlGreenMES.Modules.Tenancy.Application;
using AlGreenMES.Modules.Tenancy.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Tenancy.Api;

public static class TenancyModuleRegistration
{
    public static IServiceCollection AddTenancyModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTenancyApplication();
        services.AddTenancyInfrastructure(configuration);
        return services;
    }
}
