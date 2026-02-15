using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs.Tablet;

public record TabletQueueItemDto(
    Guid OrderItemProcessId,
    Guid OrderId,
    string OrderNumber,
    int Priority,
    DateTime DeliveryDate,
    string ProductName,
    int Quantity,
    ComplexityType? Complexity,
    ProcessStatus Status,
    List<string> SpecialRequestNames,
    int CompletedProcessCount,
    int TotalProcessCount);
