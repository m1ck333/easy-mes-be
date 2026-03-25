using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrdersMasterView;

public class GetOrdersMasterViewQueryHandler : IRequestHandler<GetOrdersMasterViewQuery, PagedResult<OrderMasterViewDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCategoryRepository _categoryRepository;

    public GetOrdersMasterViewQueryHandler(IOrderRepository orderRepository, IProductCategoryRepository categoryRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<OrderMasterViewDto>> Handle(GetOrdersMasterViewQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetPagedWithProcessesAsync(
            request.TenantId, request.Status, request.OrderType,
            request.DateFrom, request.DateTo, request.Search,
            request.SortBy, request.IsDescending,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        // Pre-load category dependencies for all categories in the result set
        var categoryIds = result.Items
            .SelectMany(o => o.Items.Select(i => i.ProductCategoryId))
            .Distinct()
            .ToList();
        var categoryDeps = new Dictionary<Guid, List<(Guid ProcessId, Guid DependsOnProcessId)>>();
        foreach (var catId in categoryIds)
        {
            var cat = await _categoryRepository.GetByIdWithDetailsAsync(catId, cancellationToken);
            if (cat?.Dependencies != null)
            {
                categoryDeps[catId] = cat.Dependencies
                    .Select(d => (d.ProcessId, d.DependsOnProcessId))
                    .ToList();
            }
        }

        return result.MapItems(o => MapToMasterView(o, categoryDeps));
    }

    private static OrderMasterViewDto MapToMasterView(Order order, Dictionary<Guid, List<(Guid ProcessId, Guid DependsOnProcessId)>> categoryDeps)
    {
        var allProcesses = order.Items.SelectMany(i => i.Processes).ToList();
        var nonWithdrawn = allProcesses.Where(p => !p.IsWithdrawn).ToList();

        var grouped = allProcesses.GroupBy(p => p.ProcessId).ToList();

        var processStatuses = grouped.ToDictionary(
            g => g.Key.ToString(),
            g => AggregateStatus(g).ToString());

        var processDurations = grouped.ToDictionary(
            g => g.Key.ToString(),
            g => g.Sum(p =>
            {
                var hasSubProcesses = p.SubProcesses.Any(sp => !sp.IsWithdrawn);
                if (hasSubProcesses)
                {
                    // Sub-process path: sum sub-process durations + any open log elapsed
                    return p.SubProcesses.Sum(sp =>
                    {
                        var spTime = sp.TotalDurationMinutes;
                        var openLog = sp.Logs?.FirstOrDefault(l => l.EndTime == null);
                        if (openLog != null)
                            spTime += (int)(DateTime.UtcNow - openLog.StartTime).TotalSeconds;
                        return spTime;
                    });
                }
                // No sub-process path: saved + live elapsed
                var saved = p.TotalDurationMinutes;
                if (p.Status == ProcessStatus.InProgress && !p.PausedAt.HasValue)
                {
                    var since = p.ResumedAt ?? p.StartedAt;
                    if (since.HasValue)
                        saved += (int)(DateTime.UtcNow - since.Value).TotalSeconds;
                }
                return saved;
            }));

        // Build process dependencies from all items' categories
        var processDependencies = new Dictionary<string, List<string>>();
        foreach (var item in order.Items)
        {
            if (categoryDeps.TryGetValue(item.ProductCategoryId, out var deps))
            {
                foreach (var dep in deps)
                {
                    var key = dep.ProcessId.ToString();
                    if (!processDependencies.ContainsKey(key))
                        processDependencies[key] = new List<string>();
                    var depKey = dep.DependsOnProcessId.ToString();
                    if (!processDependencies[key].Contains(depKey))
                        processDependencies[key].Add(depKey);
                }
            }
        }

        var completedProcesses = nonWithdrawn.Count(p => p.Status == ProcessStatus.Completed);
        var totalProcesses = nonWithdrawn.Count;

        var processPaused = grouped.ToDictionary(
            g => g.Key.ToString(),
            g =>
            {
                var inProgress = g.Where(p => p.Status == ProcessStatus.InProgress).ToList();
                if (inProgress.Count == 0) return false;
                // Paused only if ALL in-progress items for this process are paused
                return inProgress.All(p =>
                {
                    var hasSubs = p.SubProcesses.Any(sp => !sp.IsWithdrawn);
                    if (hasSubs)
                    {
                        var anyRunning = p.SubProcesses.Any(sp => sp.Status == SubProcessStatus.InProgress);
                        var allDone = p.SubProcesses.Where(sp => !sp.IsWithdrawn).All(sp => sp.Status == SubProcessStatus.Completed);
                        return !anyRunning && !allDone;
                    }
                    return p.PausedAt.HasValue;
                });
            });

        return new OrderMasterViewDto(
            order.Id,
            order.OrderNumber,
            order.OrderType.ToString(),
            order.Status.ToString(),
            order.DeliveryDate,
            order.Priority,
            order.CustomWarningDays,
            order.CustomCriticalDays,
            completedProcesses,
            totalProcesses,
            processStatuses,
            processDurations,
            processPaused,
            processDependencies,
            order.Attachments.Count,
            order.CreatedAt);
    }

    private static ProcessStatus AggregateStatus(IGrouping<Guid, OrderItemProcess> group)
    {
        var statuses = group.Select(p => p.Status).ToList();

        if (statuses.Any(s => s == ProcessStatus.Blocked))
            return ProcessStatus.Blocked;
        if (statuses.Any(s => s == ProcessStatus.Stopped))
            return ProcessStatus.Stopped;
        if (statuses.Any(s => s == ProcessStatus.InProgress))
            return ProcessStatus.InProgress;
        if (statuses.Any(s => s == ProcessStatus.Pending))
            return ProcessStatus.Pending;
        if (statuses.All(s => s == ProcessStatus.Completed))
            return ProcessStatus.Completed;
        if (statuses.All(s => s == ProcessStatus.Withdrawn))
            return ProcessStatus.Withdrawn;

        return ProcessStatus.Completed;
    }
}
