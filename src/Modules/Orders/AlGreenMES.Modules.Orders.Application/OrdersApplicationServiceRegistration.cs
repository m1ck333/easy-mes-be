using AlGreenMES.BuildingBlocks.Common.Behaviors;
using AlGreenMES.Modules.Orders.Application.Mapping;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Orders.Application;

public static class OrdersApplicationServiceRegistration
{
    public static IServiceCollection AddOrdersApplication(this IServiceCollection services)
    {
        var assembly = typeof(OrdersApplicationServiceRegistration).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        OrdersMappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}
