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

    public GetTabletIncomingQueryHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<TabletIncomingDto>> Handle(GetTabletIncomingQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var result = new List<TabletIncomingDto>();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                var category = await _categoryRepository.GetByIdWithDetailsAsync(item.ProductCategoryId, cancellationToken);
                var dependencies = category?.Dependencies ?? [];

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

                    var dto = process.Adapt<TabletIncomingDto>() with { BlockingProcesses = blockingProcesses };
                    result.Add(dto);
                }
            }
        }

        return result;
    }
}
