using AlGreenMES.Modules.Orders.Application.DTOs;
using AlGreenMES.Modules.Orders.Application.DTOs.Tablet;
using AlGreenMES.Modules.Orders.Domain.Entities;
using Mapster;

namespace AlGreenMES.Modules.Orders.Application.Mapping;

public static class OrdersMappingConfig
{
    public static void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.ItemCount, src => src.Items.Count);

        config.NewConfig<Order, OrderDetailDto>()
            .Map(dest => dest.Items, src => src.Items);

        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.Processes, src => src.Processes)
            .Map(dest => dest.SpecialRequests, src => src.SpecialRequests);

        config.NewConfig<OrderItemProcess, OrderItemProcessDto>()
            .Map(dest => dest.SubProcesses, src => src.SubProcesses);

        config.NewConfig<OrderItemSubProcess, OrderItemSubProcessDto>();
        config.NewConfig<OrderItemSpecialRequest, OrderItemSpecialRequestDto>();
        config.NewConfig<BlockRequest, BlockRequestDto>();
        config.NewConfig<ChangeRequest, ChangeRequestDto>();
        config.NewConfig<Notification, NotificationDto>();
        config.NewConfig<WorkSession, WorkSessionDto>();

        config.NewConfig<OrderItemProcess, TabletQueueItemDto>()
            .Map(dest => dest.OrderItemProcessId, src => src.Id)
            .Map(dest => dest.OrderId, src => src.OrderItem.Order.Id)
            .Map(dest => dest.OrderNumber, src => src.OrderItem.Order.OrderNumber)
            .Map(dest => dest.Priority, src => src.OrderItem.Order.Priority)
            .Map(dest => dest.DeliveryDate, src => src.OrderItem.Order.DeliveryDate)
            .Map(dest => dest.ProductName, src => src.OrderItem.ProductName)
            .Map(dest => dest.ProductCategoryName, src => (string?)null)
            .Map(dest => dest.Quantity, src => src.OrderItem.Quantity)
            .Map(dest => dest.SpecialRequestNames, src => new List<string>())
            .Map(dest => dest.CompletedProcessCount, src => 0)
            .Map(dest => dest.TotalProcessCount, src => 0);

        config.NewConfig<OrderItemProcess, TabletActiveWorkDto>()
            .Map(dest => dest.OrderItemProcessId, src => src.Id)
            .Map(dest => dest.OrderId, src => src.OrderItem.Order.Id)
            .Map(dest => dest.OrderNumber, src => src.OrderItem.Order.OrderNumber)
            .Map(dest => dest.Priority, src => src.OrderItem.Order.Priority)
            .Map(dest => dest.DeliveryDate, src => src.OrderItem.Order.DeliveryDate)
            .Map(dest => dest.ProductName, src => src.OrderItem.ProductName)
            .Map(dest => dest.ProductCategoryName, src => (string?)null)
            .Map(dest => dest.Quantity, src => src.OrderItem.Quantity)
            .Map(dest => dest.SpecialRequestNames, src => new List<string>())
            .Map(dest => dest.CompletedProcessCount, src => 0)
            .Map(dest => dest.TotalProcessCount, src => 0)
            .Map(dest => dest.SubProcesses, src => src.SubProcesses);

        config.NewConfig<OrderItemSubProcess, TabletSubProcessDto>();

        config.NewConfig<OrderItemProcess, TabletIncomingDto>()
            .Map(dest => dest.OrderItemProcessId, src => src.Id)
            .Map(dest => dest.OrderId, src => src.OrderItem.Order.Id)
            .Map(dest => dest.OrderNumber, src => src.OrderItem.Order.OrderNumber)
            .Map(dest => dest.Priority, src => src.OrderItem.Order.Priority)
            .Map(dest => dest.DeliveryDate, src => src.OrderItem.Order.DeliveryDate)
            .Map(dest => dest.ProductName, src => src.OrderItem.ProductName)
            .Map(dest => dest.ProductCategoryName, src => (string?)null)
            .Map(dest => dest.Quantity, src => src.OrderItem.Quantity)
            .Map(dest => dest.SpecialRequestNames, src => new List<string>())
            .Map(dest => dest.CompletedProcessCount, src => 0)
            .Map(dest => dest.TotalProcessCount, src => 0)
            .Map(dest => dest.BlockingProcesses, src => new List<BlockingProcessDto>());

        config.NewConfig<OrderItemProcess, BlockingProcessDto>()
            .Map(dest => dest.OrderItemProcessId, src => src.Id);
    }
}
