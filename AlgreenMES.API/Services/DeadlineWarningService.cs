using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using AlGreenMES.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlgreenMES.API.Services;

public class DeadlineWarningService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeadlineWarningService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

    public DeadlineWarningService(IServiceScopeFactory scopeFactory, ILogger<DeadlineWarningService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DeadlineWarningService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckDeadlinesAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown — exit the loop silently
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking deadlines.");
                await Task.Delay(TimeSpan.FromMinutes(1), CancellationToken.None);
            }
        }
    }

    private async Task CheckDeadlinesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var tenancyDb = scope.ServiceProvider.GetRequiredService<TenancyDbContext>();
        var ordersDb = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IOrdersUnitOfWork>();
        var eventService = scope.ServiceProvider.GetRequiredService<IProductionEventService>();

        var tenants = await tenancyDb.Tenants
            .AsNoTracking()
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var tenant in tenants)
        {
            var settings = await tenancyDb.TenantSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.TenantId == tenant.Id, cancellationToken);

            var defaultWarningDays = settings?.DefaultWarningDays ?? 7;
            var defaultCriticalDays = settings?.DefaultCriticalDays ?? 3;

            var activeOrders = await ordersDb.Orders
                .AsNoTracking()
                .Where(o => o.TenantId == tenant.Id && o.Status == OrderStatus.Active)
                .ToListAsync(cancellationToken);

            var today = DateTime.UtcNow.Date;

            foreach (var order in activeOrders)
            {
                var warningDays = order.CustomWarningDays ?? defaultWarningDays;
                var criticalDays = order.CustomCriticalDays ?? defaultCriticalDays;
                var daysRemaining = (order.DeliveryDate.Date - today).Days;

                if (daysRemaining > warningDays) continue;

                var level = daysRemaining <= criticalDays ? "Critical" : "Warning";
                var notificationType = daysRemaining <= criticalDays
                    ? NotificationType.DeadlineCritical
                    : NotificationType.DeadlineWarning;

                // Check if we already sent this level of notification today
                var alreadyNotified = await ordersDb.Notifications
                    .AnyAsync(n => n.TenantId == tenant.Id
                        && n.ReferenceType == "Order"
                        && n.ReferenceId == order.Id
                        && n.Type == notificationType
                        && n.CreatedAt.Date == today,
                        cancellationToken);

                if (alreadyNotified) continue;

                // Create notification (use a placeholder admin user - in production, notify all admins/managers)
                var notification = Notification.Create(
                    tenant.Id,
                    Guid.Empty, // broadcast notification
                    notificationType,
                    $"{level} Deadline: {order.OrderNumber}",
                    $"Order {order.OrderNumber} has {daysRemaining} days remaining until delivery ({order.DeliveryDate:yyyy-MM-dd}).",
                    "Order",
                    order.Id);

                await notificationRepository.AddAsync(notification, cancellationToken);

                // Broadcast SignalR event
                await eventService.NotifyDeadlineWarningAsync(new DeadlineWarningEvent(
                    order.Id,
                    order.OrderNumber,
                    order.DeliveryDate,
                    daysRemaining,
                    level,
                    tenant.Id), cancellationToken);

                _logger.LogInformation(
                    "{Level} deadline warning for order {OrderNumber}: {DaysRemaining} days remaining.",
                    level, order.OrderNumber, daysRemaining);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
