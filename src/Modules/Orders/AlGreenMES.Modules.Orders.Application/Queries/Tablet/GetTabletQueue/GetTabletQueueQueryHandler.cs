using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Identity.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletQueue;

public class GetTabletQueueQueryHandler : IRequestHandler<GetTabletQueueQuery, IReadOnlyList<ProcessGroupDto<TabletQueueItemDto>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly ISpecialRequestTypeRepository _specialRequestTypeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProcessRepository _processRepository;

    public GetTabletQueueQueryHandler(
        IOrderRepository orderRepository,
        IProductCategoryRepository categoryRepository,
        ISpecialRequestTypeRepository specialRequestTypeRepository,
        IUserRepository userRepository,
        IProcessRepository processRepository)
    {
        _orderRepository = orderRepository;
        _categoryRepository = categoryRepository;
        _specialRequestTypeRepository = specialRequestTypeRepository;
        _userRepository = userRepository;
        _processRepository = processRepository;
    }

    public async Task<IReadOnlyList<ProcessGroupDto<TabletQueueItemDto>>> Handle(GetTabletQueueQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithProcessesAsync(request.UserId, cancellationToken);
        if (user == null) return [];

        var userProcessIds = user.GetProcessIds();
        if (userProcessIds.Count == 0) return [];

        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var specialRequestTypes = await _specialRequestTypeRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);
        var srLookup = specialRequestTypes.ToDictionary(s => s.Id, s => s.Name);

        var result = new List<ProcessGroupDto<TabletQueueItemDto>>();

        foreach (var processId in userProcessIds)
        {
            var prodProcess = await _processRepository.GetByIdAsync(processId, cancellationToken);
            if (prodProcess == null) continue;

            var items = new List<TabletQueueItemDto>();

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
                        if (process.ProcessId != processId) continue;
                        if (process.Status != ProcessStatus.Pending) continue;
                        if (process.IsWithdrawn) continue;

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
                        items.Add(dto);
                    }
                }
            }

            items = items
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.DeliveryDate)
                .ToList();

            result.Add(new ProcessGroupDto<TabletQueueItemDto>(
                prodProcess.Id, prodProcess.Code, prodProcess.Name, prodProcess.SequenceOrder, items));
        }

        return result.OrderBy(g => g.SequenceOrder).ToList();
    }
}
