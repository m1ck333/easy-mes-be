namespace AlGreenMES.Modules.Orders.Application.DTOs.Reports;

public record ProcessAveragesDto(List<ProcessAverageItemDto> Processes);

public record ProcessAverageItemDto(
    Guid ProcessId,
    string ProcessCode,
    string ProcessName,
    Dictionary<string, ComplexityAverageDto> Averages);

public record ComplexityAverageDto(double AvgMinutes, int Count);
