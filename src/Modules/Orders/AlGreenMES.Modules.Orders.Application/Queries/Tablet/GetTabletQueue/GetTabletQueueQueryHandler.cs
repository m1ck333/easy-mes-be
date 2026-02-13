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

    public GetTabletQueueQueryHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<TabletQueueItemDto>> Handle(GetTabletQueueQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var result = new List<TabletQueueItemDto>();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                // Load category dependencies to check if process dependencies are met
                var category = await _categoryRepository.GetByIdWithDetailsAsync(item.ProductCategoryId, cancellationToken);
                var dependencies = category?.Dependencies ?? [];

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

                    result.Add(process.Adapt<TabletQueueItemDto>());
                }
            }
        }

        return result;
    }
}
