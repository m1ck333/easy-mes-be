namespace AlGreenMES.Modules.Orders.Application.DTOs.Tablet;

public record ProcessGroupDto<T>(
    Guid ProcessId,
    string ProcessCode,
    string ProcessName,
    int SequenceOrder,
    List<T> Items);
