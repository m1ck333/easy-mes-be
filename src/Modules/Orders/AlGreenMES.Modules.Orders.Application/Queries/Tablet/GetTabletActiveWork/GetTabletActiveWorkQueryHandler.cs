using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.Tablet.GetTabletActiveWork;

public class GetTabletActiveWorkQueryHandler : IRequestHandler<GetTabletActiveWorkQuery, IReadOnlyList<TabletActiveWorkDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetTabletActiveWorkQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<TabletActiveWorkDto>> Handle(GetTabletActiveWorkQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetActiveOrdersWithProcessesAsync(request.TenantId, cancellationToken);

        var result = new List<TabletActiveWorkDto>();

        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                foreach (var process in item.Processes)
                {
                    if (process.ProcessId != request.ProcessId) continue;
                    if (process.Status != ProcessStatus.InProgress) continue;

                    result.Add(process.Adapt<TabletActiveWorkDto>());
                }
            }
        }

        return result;
    }
}
