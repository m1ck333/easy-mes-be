using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Identity.Application;

public static class IdentityApplicationServiceRegistration
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IdentityApplicationServiceRegistration).Assembly));
        return services;
    }
}
