using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Tenancy.Application;

public static class TenancyApplicationServiceRegistration
{
    public static IServiceCollection AddTenancyApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TenancyApplicationServiceRegistration).Assembly));
        return services;
    }
}
