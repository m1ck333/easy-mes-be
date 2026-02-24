using AlGreenMES.Modules.Orders.Api.Hubs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AlgreenMES.API.Services;

public class ProductionEventService : IProductionEventService
{
    private readonly IHubContext<ProductionHub> _hubContext;
    private readonly IWebPushService _webPushService;

    public ProductionEventService(IHubContext<ProductionHub> hubContext, IWebPushService webPushService)
    {
        _hubContext = hubContext;
        _webPushService = webPushService;
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
}
