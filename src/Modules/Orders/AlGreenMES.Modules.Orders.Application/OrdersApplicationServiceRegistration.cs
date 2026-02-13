using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Orders.Application;

public static class OrdersApplicationServiceRegistration
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(OrdersApplicationServiceRegistration).Assembly));
        return services;
    }
}
