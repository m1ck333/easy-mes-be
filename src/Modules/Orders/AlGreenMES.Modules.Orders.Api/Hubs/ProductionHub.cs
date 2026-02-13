using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AlGreenMES.Modules.Orders.Api.Hubs;

[Authorize]
public class ProductionHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
        }

        var processId = Context.User?.FindFirst("process_id")?.Value;
        if (!string.IsNullOrEmpty(processId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"process-{processId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
        }

        var processId = Context.User?.FindFirst("process_id")?.Value;
        if (!string.IsNullOrEmpty(processId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"process-{processId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinTenantGroup(string tenantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
    }

    public async Task LeaveTenantGroup(string tenantId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
    }

    public async Task JoinProcessGroup(string processId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"process-{processId}");
    }

    public async Task LeaveProcessGroup(string processId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"process-{processId}");
    }
}
