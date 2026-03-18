using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace AlGreenMES.Modules.Orders.Api.Hubs;

[Authorize]
public class ProductionHub : Hub
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ProductionHub(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task OnConnectedAsync()
    {
        var tenantId = Context.User?.FindFirst("tenant_id")?.Value;
        if (!string.IsNullOrEmpty(tenantId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant-{tenantId}");
        }

        // Look up user's assigned processes from DB and join each group
        var userId = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
        {
            using var scope = _scopeFactory.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetByIdWithProcessesAsync(userGuid);
            if (user != null)
            {
                foreach (var processId in user.GetProcessIds())
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"process-{processId}");
                }
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Groups are automatically cleaned up by SignalR on disconnect
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
