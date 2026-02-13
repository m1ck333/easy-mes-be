using AlGreenMES.BuildingBlocks.Common.Behaviors;
using AlGreenMES.Modules.Identity.Application.Mapping;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Identity.Application;

public static class IdentityApplicationServiceRegistration
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        var assembly = typeof(IdentityApplicationServiceRegistration).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        IdentityMappingConfig.Register(TypeAdapterConfig.GlobalSettings);

        return services;
    }
}
