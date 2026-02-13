using AlGreenMES.Modules.Production.Application;
using AlGreenMES.Modules.Production.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Production.Api;

public static class ProductionModuleRegistration
{
    public static IServiceCollection AddProductionModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProductionApplication();
        services.AddProductionInfrastructure(configuration);
        return services;
    }
}
