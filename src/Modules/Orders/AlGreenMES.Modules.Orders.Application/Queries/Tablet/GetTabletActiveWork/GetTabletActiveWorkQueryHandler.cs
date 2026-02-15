using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletActiveWork;

public class GetTabletActiveWorkQueryHandler : IRequestHandler<GetTabletActiveWorkQuery, IReadOnlyList<TabletActiveWorkDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISpecialRequestTypeRepository _specialRequestTypeRepository;

    public GetTabletActiveWorkQueryHandler(
        IOrderRepository orderRepository,
        ISpecialRequestTypeRepository specialRequestTypeRepository)
    {
        _orderRepository = orderRepository;
        _specialRequestTypeRepository = specialRequestTypeRepository;
    }

    public async Task<IReadOnlyList<TabletActiveWorkDto>> Handle(GetTabletActiveWorkQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var specialRequestTypes = await _specialRequestTypeRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);
        var srLookup = specialRequestTypes.ToDictionary(s => s.Id, s => s.Name);

        var result = new List<TabletActiveWorkDto>();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
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
                    if (process.Status != ProcessStatus.InProgress) continue;

                    var dto = process.Adapt<TabletActiveWorkDto>() with
                    {
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
