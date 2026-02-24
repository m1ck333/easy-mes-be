using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Entities;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrdersMasterView;

public class GetOrdersMasterViewQueryHandler : IRequestHandler<GetOrdersMasterViewQuery, PagedResult<OrderMasterViewDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersMasterViewQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderMasterViewDto>> Handle(GetOrdersMasterViewQuery request, CancellationToken cancellationToken)
    {
        var result = await _orderRepository.GetPagedWithProcessesAsync(
            request.TenantId, request.Status, request.OrderType,
            request.DateFrom, request.DateTo, request.Search,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(MapToMasterView);
    }

    private static OrderMasterViewDto MapToMasterView(Order order)
    {
        var allProcesses = order.Items.SelectMany(i => i.Processes).ToList();
        var nonWithdrawn = allProcesses.Where(p => !p.IsWithdrawn).ToList();

        var processStatuses = allProcesses
            .GroupBy(p => p.ProcessId)
            .ToDictionary(
                g => g.Key.ToString(),
                g => AggregateStatus(g).ToString());

        var completedProcesses = nonWithdrawn.Count(p => p.Status == ProcessStatus.Completed);
        var totalProcesses = nonWithdrawn.Count;

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
            processStatuses);
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
