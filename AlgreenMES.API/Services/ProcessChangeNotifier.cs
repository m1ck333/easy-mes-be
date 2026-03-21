using AlGreenMES.BuildingBlocks.Common.Interfaces;
using AlGreenMES.Modules.Orders.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AlgreenMES.API.Services;

public class ProcessChangeNotifier : IProcessChangeNotifier
{
    private readonly IHubContext<ProductionHub> _hubContext;

    public ProcessChangeNotifier(IHubContext<ProductionHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task NotifyProcessDefinitionChangedAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return _hubContext.Clients.Group($"tenant-{tenantId}")
            .SendAsync("ProcessDefinitionUpdated", new { TenantId = tenantId }, cancellationToken);
    }
}
