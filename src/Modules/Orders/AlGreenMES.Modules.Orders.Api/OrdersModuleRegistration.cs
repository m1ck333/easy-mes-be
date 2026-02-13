using AlGreenMES.Modules.Orders.Application;
using AlGreenMES.Modules.Orders.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Orders.Api;

public static class OrdersModuleRegistration
{
    public static IServiceCollection AddOrdersModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOrdersApplication();
        services.AddOrdersInfrastructure(configuration);
        services.AddSignalR();
        return services;
    }
}
