using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
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

                    result.Add(new TabletActiveWorkDto(
                        process.Id,
                        order.Id,
                        order.OrderNumber,
                        order.Priority,
                        order.DeliveryDate,
                        item.ProductName,
                        item.Quantity,
                        process.Complexity,
                        process.Status,
                        process.StartedAt,
                        process.TotalDurationMinutes,
                        process.SubProcesses.Select(sp => new TabletSubProcessDto(
                            sp.Id,
                            sp.SubProcessId,
                            sp.Status,
                            sp.TotalDurationMinutes,
                            sp.IsWithdrawn)).ToList()));
                }
            }
        }

        return result;
    }
}
