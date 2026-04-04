using AlGreenMES.Modules.Identity.Infrastructure.Persistence;
using AlGreenMES.Modules.Orders.Application.DTOs.Reports;
using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Infrastructure.Persistence;
using AlGreenMES.Modules.Production.Domain.Enums;
using AlGreenMES.Modules.Production.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Orders.Infrastructure.Services;

public class ReportingQueryService : IReportingQueryService
{
    private readonly OrdersDbContext _ordersDb;
    private readonly ProductionDbContext _productionDb;
    private readonly IdentityDbContext _identityDb;

    public ReportingQueryService(
        OrdersDbContext ordersDb,
        ProductionDbContext productionDb,
        IdentityDbContext identityDb)
    {
        _ordersDb = ordersDb;
        _productionDb = productionDb;
        _identityDb = identityDb;
    }

    public async Task<ProcessAveragesDto> GetProcessAveragesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var processes = await _productionDb.Processes
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .OrderBy(p => p.SequenceOrder)
            .Select(p => new { p.Id, p.Code, p.Name })
            .ToListAsync(cancellationToken);

        var completedProcesses = await _ordersDb.OrderItemProcesses
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId
                && p.Status == ProcessStatus.Completed
                && p.Complexity.HasValue
                && p.TotalDurationMinutes > 0)
            .Select(p => new { p.ProcessId, p.Complexity, p.TotalDurationMinutes })
            .ToListAsync(cancellationToken);

        var grouped = completedProcesses
            .GroupBy(p => new { p.ProcessId, p.Complexity })
            .ToDictionary(
                g => g.Key,
                g => new ComplexityAverageDto(
                    Math.Round(g.Average(x => x.TotalDurationMinutes), 1),
                    g.Count()));

        var result = new List<ProcessAverageItemDto>();

        foreach (var process in processes)
        {
            var averages = new Dictionary<string, ComplexityAverageDto>();

            foreach (var complexity in Enum.GetValues<ComplexityType>())
            {
                var key = new { ProcessId = process.Id, Complexity = (ComplexityType?)complexity };
                if (grouped.TryGetValue(key, out var avg))
                {
                    averages[complexity.ToString()] = avg;
                }
            }

            result.Add(new ProcessAverageItemDto(process.Id, process.Code, process.Name, averages));
        }

        return new ProcessAveragesDto(result);
    }

    public async Task<TimeTrackingReportDto> GetTimeTrackingReportAsync(
        Guid tenantId,
        DateTime? from,
        DateTime? to,
        Guid? processId,
        ComplexityType? complexity,
        CancellationToken cancellationToken = default)
    {
        var processes = await _productionDb.Processes
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId)
            .ToDictionaryAsync(p => p.Id, p => new { p.Code, p.Name }, cancellationToken);

        var query = _ordersDb.OrderItemProcesses
            .AsNoTracking()
            .Include(p => p.OrderItem)
                .ThenInclude(oi => oi.Order)
            .Where(p => p.TenantId == tenantId
                && p.Status == ProcessStatus.Completed
                && p.TotalDurationMinutes > 0);

        if (from.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(from.Value.Date, DateTimeKind.Utc);
            query = query.Where(p => p.CompletedAt >= fromUtc);
        }

        if (to.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(to.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            query = query.Where(p => p.CompletedAt <= toUtc);
        }

        if (processId.HasValue)
            query = query.Where(p => p.ProcessId == processId.Value);

        if (complexity.HasValue)
            query = query.Where(p => p.Complexity == complexity.Value);

        var data = await query
            .OrderByDescending(p => p.CompletedAt)
            .ToListAsync(cancellationToken);

        var items = data.Select(p =>
        {
            var proc = processes.GetValueOrDefault(p.ProcessId);
            return new TimeTrackingItemDto(
                p.Id,
                p.OrderItem?.Order?.OrderNumber ?? "—",
                p.OrderItem?.ProductName ?? "—",
                p.ProcessId,
                proc?.Code ?? "?",
                proc?.Name ?? "Unknown",
                p.Complexity?.ToString(),
                p.Status.ToString(),
                p.StartedAt,
                p.CompletedAt,
                p.TotalDurationMinutes);
        }).ToList();

        var summary = items.Count > 0
            ? new TimeTrackingSummaryDto(
                items.Count,
                Math.Round(items.Average(i => i.TotalDurationMinutes), 1),
                items.Sum(i => i.TotalDurationMinutes),
                items.Min(i => i.TotalDurationMinutes),
                items.Max(i => i.TotalDurationMinutes))
            : new TimeTrackingSummaryDto(0, 0, 0, 0, 0);

        return new TimeTrackingReportDto(items, summary);
    }

    public async Task<WorkerHoursReportDto> GetWorkerHoursReportAsync(
        Guid tenantId,
        DateOnly from,
        DateOnly to,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        var sessionsQuery = _ordersDb.WorkSessions
            .AsNoTracking()
            .Where(ws => ws.TenantId == tenantId
                && ws.Date >= from
                && ws.Date <= to
                && ws.CheckOutTime.HasValue
                && ws.DurationMinutes.HasValue);

        if (userId.HasValue)
            sessionsQuery = sessionsQuery.Where(ws => ws.UserId == userId.Value);

        var sessions = await sessionsQuery.ToListAsync(cancellationToken);

        var userIds = sessions.Select(s => s.UserId).Distinct().ToList();
        var users = await _identityDb.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}", cancellationToken);

        var grouped = sessions
            .GroupBy(s => s.UserId)
            .Select(g =>
            {
                var dailyBreakdown = g
                    .GroupBy(s => s.Date)
                    .OrderBy(d => d.Key)
                    .Select(d => new WorkerHoursDayDto(
                        d.Key,
                        d.Sum(s => s.DurationMinutes ?? 0),
                        d.Count()))
                    .ToList();

                return new WorkerHoursSummaryDto(
                    g.Key,
                    users.GetValueOrDefault(g.Key, "Unknown"),
                    g.Sum(s => s.DurationMinutes ?? 0),
                    g.Count(),
                    dailyBreakdown);
            })
            .OrderBy(w => w.FullName)
            .ToList();

        return new WorkerHoursReportDto(grouped);
    }
}
