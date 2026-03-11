using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletIncoming;

public class GetTabletIncomingQueryHandler : IRequestHandler<GetTabletIncomingQuery, IReadOnlyList<TabletIncomingDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly ISpecialRequestTypeRepository _specialRequestTypeRepository;

    public GetTabletIncomingQueryHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository,
        ISpecialRequestTypeRepository specialRequestTypeRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
        _specialRequestTypeRepository = specialRequestTypeRepository;
    }

    public async Task<IReadOnlyList<TabletIncomingDto>> Handle(GetTabletIncomingQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var specialRequestTypes = await _specialRequestTypeRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);
        var srLookup = specialRequestTypes.ToDictionary(s => s.Id, s => s.Name);

        var result = new List<TabletIncomingDto>();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
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

                    var processDeps = dependencies
                        .Where(d => d.ProcessId == process.ProcessId)
                        .Select(d => d.DependsOnProcessId)
                        .ToList();

                    // Only include items with unmet dependencies
                    var blockingProcesses = new List<BlockingProcessDto>();
                    foreach (var depProcessId in processDeps)
                    {
                        var depProcess = item.Processes.FirstOrDefault(p => p.ProcessId == depProcessId);
                        if (depProcess != null && depProcess.Status != ProcessStatus.Completed && depProcess.Status != ProcessStatus.Withdrawn)
                        {
                            blockingProcesses.Add(depProcess.Adapt<BlockingProcessDto>());
                        }
                    }

                    if (blockingProcesses.Count == 0) continue;

                    var dto = process.Adapt<TabletIncomingDto>() with
                    {
                        ProductCategoryName = category?.Name,
                        SpecialRequestNames = specialRequestNames,
                        CompletedProcessCount = completedCount,
                        TotalProcessCount = totalCount,
                        BlockingProcesses = blockingProcesses
                    };
                    result.Add(dto);
                }
            }
        }

        return result;
    }
}
