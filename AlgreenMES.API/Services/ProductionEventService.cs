using AlGreenMES.Modules.Orders.Api.Hubs;
using AlGreenMES.Modules.Orders.Application.DTOs.Events;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace AlgreenMES.API.Services;

public class ProductionEventService : IProductionEventService
{
    private readonly IHubContext<ProductionHub> _hubContext;

    public ProductionEventService(IHubContext<ProductionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyOrderActivatedAsync(OrderActivatedEvent evt, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group($"tenant-{evt.TenantId}")
            .SendAsync("OrderActivated", evt, cancellationToken);
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
    }
}
