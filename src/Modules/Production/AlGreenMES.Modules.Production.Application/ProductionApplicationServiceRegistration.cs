using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Production.Application;

public static class ProductionApplicationServiceRegistration
{
    public static IServiceCollection AddProductionApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ProductionApplicationServiceRegistration).Assembly));
        return services;
    }
}
