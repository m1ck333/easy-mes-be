using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using AlGreenMES.Modules.Orders.Infrastructure.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AlGreenMES.Modules.Orders.Infrastructure;

public static class OrdersInfrastructureServiceRegistration
{
    public static IServiceCollection AddOrdersInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrdersDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            options.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IOrdersUnitOfWork>(sp => sp.GetRequiredService<OrdersDbContext>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemProcessRepository, OrderItemProcessRepository>();
        services.AddScoped<IOrderItemSubProcessRepository, OrderItemSubProcessRepository>();
        services.AddScoped<IWorkSessionRepository, WorkSessionRepository>();
        services.AddScoped<IChangeRequestRepository, ChangeRequestRepository>();
        services.AddScoped<IBlockRequestRepository, BlockRequestRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPushSubscriptionRepository, PushSubscriptionRepository>();
        services.AddScoped<IOrderAttachmentRepository, OrderAttachmentRepository>();

        // File Storage
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // WebPush
        services.Configure<WebPushSettings>(configuration.GetSection("WebPush"));
        services.AddScoped<IWebPushService, WebPushService>();

        services.AddScoped<IDashboardQueryService, DashboardQueryService>();

        return services;
    }
}
