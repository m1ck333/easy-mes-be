using AlGreenMES.BuildingBlocks.Common.Behaviors;
using AlGreenMES.Modules.Tenancy.Application.Mapping;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Tenancy.Application;

public static class TenancyApplicationServiceRegistration
{
    public static IServiceCollection AddTenancyApplication(this IServiceCollection services)
    {
        var assembly = typeof(TenancyApplicationServiceRegistration).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        TenancyMappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}
