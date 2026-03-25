using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderAttachmentRepository _attachmentRepository;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IOrderAttachmentRepository attachmentRepository)
    {
        _orderRepository = orderRepository;
        _attachmentRepository = attachmentRepository;
    }

    public async Task<OrderDetailDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithFullDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        var attachments = await _attachmentRepository.GetByOrderIdAsync(request.Id, cancellationToken);

        var dto = order.Adapt<OrderDetailDto>();

        // Enrich sub-process DTOs with timer info from logs
        var enrichedItems = dto.Items.Select(item =>
        {
            var orderItem = order.Items.First(i => i.Id == item.Id);
            var enrichedProcesses = item.Processes.Select(proc =>
            {
                var entity = orderItem.Processes.First(p => p.Id == proc.Id);
                var enrichedSubs = proc.SubProcesses.Select(sub =>
                {
                    var subEntity = entity.SubProcesses.First(s => s.Id == sub.Id);
                    var openLog = subEntity.GetOpenLog();
                    return sub with
                    {
                        IsTimerRunning = openLog != null,
                        CurrentLogStartedAt = openLog?.StartTime
                    };
                }).ToList();
                return proc with { SubProcesses = enrichedSubs };
            }).ToList();
            return item with { Processes = enrichedProcesses };
        }).ToList();
        dto = dto with { Items = enrichedItems };

        // Set order-level attachments
        var orderLevelAttachments = attachments
            .Where(a => a.OrderItemId == null)
            .Select(a => a.Adapt<OrderAttachmentDto>())
            .ToList();

        // Distribute item-level attachments to each item DTO
        var itemAttachments = attachments
            .Where(a => a.OrderItemId != null)
            .GroupBy(a => a.OrderItemId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(a => a.Adapt<OrderAttachmentDto>()).ToList());

        var updatedItems = dto.Items.Select(item =>
            item with { Attachments = itemAttachments.GetValueOrDefault(item.Id, []) }
        ).ToList();

        return dto with { Items = updatedItems, Attachments = orderLevelAttachments };
    }
}
