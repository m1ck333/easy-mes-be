using AlGreenMES.Modules.Identity.Infrastructure.Persistence;
using AlGreenMES.Modules.Orders.Application.DTOs.Dashboard;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using AlGreenMES.Modules.Tenancy.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Services;

public class DashboardQueryService : IDashboardQueryService
{
    private readonly OrdersDbContext _ordersDb;
    private readonly ProductionDbContext _productionDb;
    private readonly IdentityDbContext _identityDb;
    private readonly TenancyDbContext _tenancyDb;

    public DashboardQueryService(
        OrdersDbContext ordersDb,
        ProductionDbContext productionDb,
        IdentityDbContext identityDb,
        TenancyDbContext tenancyDb)
    {
        _ordersDb = ordersDb;
        _productionDb = productionDb;
        _identityDb = identityDb;
        _tenancyDb = tenancyDb;
    }

    public async Task<IReadOnlyList<DeadlineWarningDto>> GetDeadlineWarningsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var settings = await _tenancyDb.TenantSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, cancellationToken);

        var defaultWarningDays = settings?.DefaultWarningDays ?? 7;
        var defaultCriticalDays = settings?.DefaultCriticalDays ?? 3;

        var processes = await _productionDb.Processes
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId)
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);

        var activeOrders = await _ordersDb.Orders
            .AsNoTracking()
            .Include(o => o.Items)
                .ThenInclude(i => i.Processes)
            .Where(o => o.TenantId == tenantId && o.Status == OrderStatus.Active)
            .ToListAsync(cancellationToken);

        var warnings = new List<DeadlineWarningDto>();

        foreach (var order in activeOrders)
        {
            var warningDays = order.CustomWarningDays ?? defaultWarningDays;
            var criticalDays = order.CustomCriticalDays ?? defaultCriticalDays;
            var daysRemaining = (order.DeliveryDate.Date - DateTime.UtcNow.Date).Days;

            if (daysRemaining > warningDays) continue;

            var level = daysRemaining <= criticalDays ? "Critical" : "Warning";

            string? currentProcess = null;
            var inProgressProcess = order.Items
                .SelectMany(i => i.Processes)
                .FirstOrDefault(p => p.Status == ProcessStatus.InProgress);
            if (inProgressProcess != null && processes.TryGetValue(inProgressProcess.ProcessId, out var processName))
                currentProcess = processName;

            warnings.Add(new DeadlineWarningDto(
                order.Id,
                order.OrderNumber,
                order.DeliveryDate,
                daysRemaining,
                level,
                order.Status.ToString(),
                currentProcess));
        }

        return warnings.OrderBy(w => w.DaysRemaining).ToList();
    }

    public async Task<IReadOnlyList<LiveViewProcessDto>> GetLiveViewAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var processes = await _productionDb.Processes
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .OrderBy(p => p.SequenceOrder)
            .ToListAsync(cancellationToken);

        var activeOrderItemProcesses = await _ordersDb.OrderItemProcesses
            .AsNoTracking()
            .Include(oip => oip.OrderItem)
                .ThenInclude(oi => oi.Order)
            .Where(oip => oip.TenantId == tenantId
                && (oip.Status == ProcessStatus.Pending || oip.Status == ProcessStatus.InProgress || oip.Status == ProcessStatus.Blocked)
                && oip.OrderItem.Order.Status == OrderStatus.Active)
            .ToListAsync(cancellationToken);

        var result = new List<LiveViewProcessDto>();

        foreach (var process in processes)
        {
            var processItems = activeOrderItemProcesses
                .Where(oip => oip.ProcessId == process.Id)
                .ToList();

            var activeOrders = processItems.Select(oip => new LiveViewOrderDto(
                oip.OrderItem.Order.Id,
                oip.OrderItem.Order.OrderNumber,
                oip.OrderItemId,
                oip.OrderItem.ProductName,
                oip.Status.ToString(),
                oip.Status == ProcessStatus.Blocked,
                oip.BlockReason)).ToList();

            result.Add(new LiveViewProcessDto(
                process.Id,
                process.Code,
                process.Name,
                activeOrders,
                processItems.Count(p => p.Status == ProcessStatus.Pending),
                processItems.Count(p => p.Status == ProcessStatus.InProgress)));
        }

        return result;
    }

    public async Task<IReadOnlyList<WorkerStatusDto>> GetWorkersStatusAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var processes = await _productionDb.Processes
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .ToDictionaryAsync(p => p.Id, p => p.Code, cancellationToken);

        var departmentUsers = await _identityDb.Users
            .AsNoTracking()
            .Include(u => u.UserProcesses)
            .Where(u => u.TenantId == tenantId && u.Role == Identity.Domain.Entities.UserRole.Department && u.IsActive)
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var activeSessions = await _ordersDb.WorkSessions
            .AsNoTracking()
            .Where(ws => ws.TenantId == tenantId && ws.CheckOutTime == null && ws.Date == today)
            .ToDictionaryAsync(ws => ws.UserId, ws => ws, cancellationToken);

        var result = new List<WorkerStatusDto>();

        foreach (var user in departmentUsers)
        {
            var session = activeSessions.GetValueOrDefault(user.Id);
            var assignedProcessCodes = user.UserProcesses
                .Select(up => processes.GetValueOrDefault(up.ProcessId, "?"))
                .OrderBy(code => code)
                .ToList();

            result.Add(new WorkerStatusDto(
                user.Id,
                $"{user.FirstName} {user.LastName}",
                session != null,
                session?.CheckInTime,
                assignedProcessCodes));
        }

        return result;
    }

    public async Task<IReadOnlyList<PendingBlockRequestDto>> GetPendingBlockRequestsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var pendingRequests = await _ordersDb.BlockRequests
            .AsNoTracking()
            .Include(br => br.OrderItemProcess)
                .ThenInclude(oip => oip!.OrderItem)
                    .ThenInclude(oi => oi.Order)
            .Where(br => br.TenantId == tenantId && br.Status == RequestStatus.Pending)
            .OrderBy(br => br.CreatedAt)
            .ToListAsync(cancellationToken);

        var processIds = pendingRequests
            .Where(br => br.OrderItemProcess != null)
            .Select(br => br.OrderItemProcess!.ProcessId)
            .Distinct()
            .ToList();

        var processes = await _productionDb.Processes
            .AsNoTracking()
            .Where(p => processIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);

        var userIds = pendingRequests.Select(br => br.RequestedByUserId).Distinct().ToList();
        var users = await _identityDb.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        return pendingRequests.Select(br =>
        {
            var processName = br.OrderItemProcess != null && processes.TryGetValue(br.OrderItemProcess.ProcessId, out var name)
                ? name : "Unknown";
            var orderNumber = br.OrderItemProcess?.OrderItem?.Order?.OrderNumber ?? "Unknown";
            var productName = br.OrderItemProcess?.OrderItem?.ProductName ?? "Unknown";
            var requestedBy = users.TryGetValue(br.RequestedByUserId, out var userName) ? userName : "Unknown";

            return new PendingBlockRequestDto(
                br.Id,
                orderNumber,
                processName,
                productName,
                requestedBy,
                br.CreatedAt,
                br.RequestNote);
        }).ToList();
    }

    public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        var ordersCompletedToday = await _ordersDb.Orders
            .AsNoTracking()
            .CountAsync(o => o.TenantId == tenantId
                && o.Status == OrderStatus.Completed
                && o.UpdatedAt.HasValue && o.UpdatedAt.Value.Date == today, cancellationToken);

        var ordersActive = await _ordersDb.Orders
            .AsNoTracking()
            .CountAsync(o => o.TenantId == tenantId && o.Status == OrderStatus.Active, cancellationToken);

        var processesCompletedToday = await _ordersDb.OrderItemProcesses
            .AsNoTracking()
            .CountAsync(p => p.TenantId == tenantId
                && p.Status == ProcessStatus.Completed
                && p.CompletedAt.HasValue && p.CompletedAt.Value.Date == today, cancellationToken);

        var completedProcessesToday = await _ordersDb.OrderItemProcesses
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId
                && p.Status == ProcessStatus.Completed
                && p.CompletedAt.HasValue && p.CompletedAt.Value.Date == today
                && p.TotalDurationMinutes > 0)
            .Select(p => p.TotalDurationMinutes)
            .ToListAsync(cancellationToken);

        var avgProcessTime = completedProcessesToday.Count > 0
            ? completedProcessesToday.Average()
            : 0;

        // Warnings count
        var settings = await _tenancyDb.TenantSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId, cancellationToken);

        var defaultWarningDays = settings?.DefaultWarningDays ?? 7;
        var defaultCriticalDays = settings?.DefaultCriticalDays ?? 3;

        var activeOrders = await _ordersDb.Orders
            .AsNoTracking()
            .Where(o => o.TenantId == tenantId && o.Status == OrderStatus.Active)
            .Select(o => new { o.DeliveryDate, o.CustomWarningDays, o.CustomCriticalDays })
            .ToListAsync(cancellationToken);

        var criticalCount = 0;
        var warningCount = 0;
        foreach (var order in activeOrders)
        {
            var daysRemaining = (order.DeliveryDate.Date - today).Days;
            var criticalThreshold = order.CustomCriticalDays ?? defaultCriticalDays;
            var warningThreshold = order.CustomWarningDays ?? defaultWarningDays;

            if (daysRemaining <= criticalThreshold) criticalCount++;
            else if (daysRemaining <= warningThreshold) warningCount++;
        }

        var pendingBlockRequests = await _ordersDb.BlockRequests
            .AsNoTracking()
            .CountAsync(br => br.TenantId == tenantId && br.Status == RequestStatus.Pending, cancellationToken);

        return new DashboardStatisticsDto(
            new DashboardTodayDto(ordersCompletedToday, ordersActive, processesCompletedToday, avgProcessTime),
            new DashboardWarningsDto(criticalCount, warningCount),
            pendingBlockRequests);
    }
}
