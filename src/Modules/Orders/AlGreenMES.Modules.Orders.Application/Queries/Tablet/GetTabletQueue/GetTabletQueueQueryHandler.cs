using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletQueue;

public class GetTabletQueueQueryHandler : IRequestHandler<GetTabletQueueQuery, IReadOnlyList<TabletQueueItemDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly ISpecialRequestTypeRepository _specialRequestTypeRepository;

    public GetTabletQueueQueryHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository,
        ISpecialRequestTypeRepository specialRequestTypeRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
        _specialRequestTypeRepository = specialRequestTypeRepository;
    }

    public async Task<IReadOnlyList<TabletQueueItemDto>> Handle(GetTabletQueueQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var specialRequestTypes = await _specialRequestTypeRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);
        var srLookup = specialRequestTypes.ToDictionary(s => s.Id, s => s.Name);

        var result = new List<TabletQueueItemDto>();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                // Load category dependencies to check if process dependencies are met
                var category = await _categoryRepository.GetByIdWithDetailsAsync(item.ProductCategoryId, cancellationToken);
                var dependencies = category?.Dependencies ?? [];

                var specialRequestNames = item.SpecialRequests
                    .Select(sr => srLookup.GetValueOrDefault(sr.SpecialRequestTypeId, ""))
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                var completedCount = item.Processes.Count(p =>
                    p.Status == ProcessStatus.Completed || p.Status == ProcessStatus.Withdrawn);
                var totalCount = item.Processes.Count(p => !p.IsWithdrawn);

                foreach (var process in item.Processes)
                {
                    if (process.ProcessId != request.ProcessId) continue;
                    if (process.Status != ProcessStatus.Pending) continue;
                    if (process.IsWithdrawn) continue;

                    // Check if all dependencies are completed
                    var processDeps = dependencies
                        .Where(d => d.ProcessId == process.ProcessId)
                        .Select(d => d.DependsOnProcessId)
                        .ToList();

                    var allDepsCompleted = processDeps.All(depProcessId =>
                        item.Processes.Any(p =>
                            p.ProcessId == depProcessId &&
                            (p.Status == ProcessStatus.Completed || p.Status == ProcessStatus.Withdrawn)));

                    if (!allDepsCompleted) continue;

                    var dto = process.Adapt<TabletQueueItemDto>() with
                    {
                        ProductCategoryName = category?.Name,
                        SpecialRequestNames = specialRequestNames,
                        CompletedProcessCount = completedCount,
                        TotalProcessCount = totalCount
                    };
                    result.Add(dto);
                }
            }
        }

        return result;
    }
}
