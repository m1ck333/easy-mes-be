using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Orders.Application.DTOs.Tablet;

public record TabletIncomingDto(
    Guid OrderItemProcessId,
    Guid OrderId,
    string OrderNumber,
    int Priority,
    DateTime DeliveryDate,
    string ProductName,
    int Quantity,
    ComplexityType? Complexity,
    ProcessStatus Status,
    List<BlockingProcessDto> BlockingProcesses);

public record BlockingProcessDto(
    Guid OrderItemProcessId,
    Guid ProcessId,
    ProcessStatus Status);
