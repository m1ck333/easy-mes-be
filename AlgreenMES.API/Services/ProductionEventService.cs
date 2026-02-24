using AlGreenMES.Modules.Identity.Domain.Entities;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using AlGreenMES.Modules.Orders.Api.Hubs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace AlgreenMES.API.Services;

public class ProductionEventService : IProductionEventService
{
    private readonly IHubContext<ProductionHub> _hubContext;
    private readonly IWebPushService _webPushService;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    private static readonly UserRole[] DashboardRoles = [UserRole.Admin, UserRole.Manager, UserRole.Coordinator, UserRole.SalesManager];

    public ProductionEventService(
        IHubContext<ProductionHub> hubContext,
        IWebPushService webPushService,
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _hubContext = hubContext;
        _webPushService = webPushService;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task NotifyOrderActivatedAsync(OrderActivatedEvent evt, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"tenant-{evt.TenantId}")
            .SendAsync("OrderActivated", evt, cancellationToken);

        await _webPushService.SendToTenantAsync(evt.TenantId,
            "New Order",
            $"Order #{evt.OrderNumber} has been activated",
            new { type = "OrderActivated", evt.OrderId, evt.OrderNumber },
            cancellationToken);

        await CreateNotificationsForDashboardUsersAsync(evt.TenantId,
            NotificationType.OrderActivated,
            "New Order",
            $"Order #{evt.OrderNumber} has been activated",
            "Order", evt.OrderId, cancellationToken);
    }

    public async Task NotifyProcessStartedAsync(ProcessStartedEvent evt, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _hubContext.Clients.Group($"tenant-{evt.TenantId}")
                .SendAsync("ProcessStarted", evt, cancellationToken),
            _hubContext.Clients.Group($"process-{evt.ProcessId}")
                .SendAsync("ProcessStarted", evt, cancellationToken));
    }

    public async Task NotifyProcessCompletedAsync(ProcessCompletedEvent evt, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _hubContext.Clients.Group($"tenant-{evt.TenantId}")
                .SendAsync("ProcessCompleted", evt, cancellationToken),
            _hubContext.Clients.Group($"process-{evt.ProcessId}")
                .SendAsync("ProcessCompleted", evt, cancellationToken));

        await CreateNotificationsForDashboardUsersAsync(evt.TenantId,
            NotificationType.ProcessCompleted,
            "Process Completed",
            $"Order #{evt.OrderNumber} — a process has been completed",
            "Order", evt.OrderId, cancellationToken);
    }

    public async Task NotifyProcessBlockedAsync(ProcessBlockedEvent evt, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _hubContext.Clients.Group($"tenant-{evt.TenantId}")
                .SendAsync("ProcessBlocked", evt, cancellationToken),
            _hubContext.Clients.Group($"process-{evt.ProcessId}")
                .SendAsync("ProcessBlocked", evt, cancellationToken));

        await _webPushService.SendToTenantAsync(evt.TenantId,
            "Process Blocked",
            $"Order #{evt.OrderNumber} — process blocked: {evt.Reason}",
            new { type = "ProcessBlocked", evt.OrderItemProcessId, evt.OrderId, evt.OrderNumber },
            cancellationToken);

        await CreateNotificationsForDashboardUsersAsync(evt.TenantId,
            NotificationType.ProcessBlocked,
            "Process Blocked",
            $"Order #{evt.OrderNumber} — process blocked: {evt.Reason}",
            "Order", evt.OrderId, cancellationToken);
    }

    public async Task NotifyProcessUnblockedAsync(ProcessUnblockedEvent evt, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _hubContext.Clients.Group($"tenant-{evt.TenantId}")
                .SendAsync("ProcessUnblocked", evt, cancellationToken),
            _hubContext.Clients.Group($"process-{evt.ProcessId}")
                .SendAsync("ProcessUnblocked", evt, cancellationToken));
    }

    public async Task NotifyBlockRequestCreatedAsync(BlockRequestCreatedEvent evt, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"tenant-{evt.TenantId}")
            .SendAsync("BlockRequestCreated", evt, cancellationToken);

        await CreateNotificationsForDashboardUsersAsync(evt.TenantId,
            NotificationType.BlockRequest,
            "New Block Request",
            "A new block request has been submitted for review",
            "BlockRequest", evt.BlockRequestId, cancellationToken);
    }

    public async Task NotifyBlockRequestApprovedAsync(BlockRequestApprovedEvent evt, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"tenant-{evt.TenantId}")
            .SendAsync("BlockRequestApproved", evt, cancellationToken);

        await _webPushService.SendToTenantAsync(evt.TenantId,
            "Block Request Approved",
            "A block request has been approved",
            new { type = "BlockRequestApproved", evt.BlockRequestId },
            cancellationToken);

        await CreateNotificationsForDashboardUsersAsync(evt.TenantId,
            NotificationType.BlockRequestApproved,
            "Block Request Approved",
            "A block request has been approved",
            "BlockRequest", evt.BlockRequestId, cancellationToken);
    }

    public async Task NotifyWorkerCheckedInAsync(WorkerCheckedInEvent evt, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _hubContext.Clients.Group($"tenant-{evt.TenantId}")
                .SendAsync("WorkerCheckedIn", evt, cancellationToken),
            _hubContext.Clients.Group($"process-{evt.ProcessId}")
                .SendAsync("WorkerCheckedIn", evt, cancellationToken));
    }

    public async Task NotifyWorkerCheckedOutAsync(WorkerCheckedOutEvent evt, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(
            _hubContext.Clients.Group($"tenant-{evt.TenantId}")
                .SendAsync("WorkerCheckedOut", evt, cancellationToken),
            _hubContext.Clients.Group($"process-{evt.ProcessId}")
                .SendAsync("WorkerCheckedOut", evt, cancellationToken));
    }

    public async Task NotifyDeadlineWarningAsync(DeadlineWarningEvent evt, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"tenant-{evt.TenantId}")
            .SendAsync("DeadlineWarning", evt, cancellationToken);

        await _webPushService.SendToTenantAsync(evt.TenantId,
            $"Deadline {evt.Level}",
            $"Order #{evt.OrderNumber} — {evt.DaysRemaining} days remaining",
            new { type = "DeadlineWarning", evt.OrderId, evt.OrderNumber, evt.DaysRemaining, evt.Level },
            cancellationToken);
    }

    public async Task NotifyProcessReadyForQueueAsync(ProcessReadyForQueueEvent evt, CancellationToken cancellationToken = default)
    {
        // Send SignalR to workers on the target process
        await _hubContext.Clients.Group($"process-{evt.ProcessId}")
            .SendAsync("ProcessReadyForQueue", evt, cancellationToken);

        // Send push notification only to workers assigned to this process
        var users = await _userRepository.GetByProcessIdAsync(evt.ProcessId, evt.TenantId, cancellationToken);
        if (users.Count > 0)
        {
            var userIds = users.Select(u => u.Id);
            await _webPushService.SendToUsersAsync(userIds,
                "New Order in Queue",
                $"Order #{evt.OrderNumber} is ready for your process",
                new { type = "ProcessReadyForQueue", evt.OrderItemProcessId, evt.OrderId, evt.OrderNumber },
                cancellationToken);
        }
    }

    private async Task CreateNotificationsForDashboardUsersAsync(
        Guid tenantId,
        NotificationType type,
        string title,
        string message,
        string? referenceType,
        Guid? referenceId,
        CancellationToken cancellationToken)
    {
        var allUsers = await _userRepository.GetByTenantIdAsync(tenantId, cancellationToken);
        var dashboardUsers = allUsers.Where(u => u.IsActive && DashboardRoles.Contains(u.Role));

        foreach (var user in dashboardUsers)
        {
            var notification = Notification.Create(tenantId, user.Id, type, title, message, referenceType, referenceId);
            await _notificationRepository.AddAsync(notification, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
