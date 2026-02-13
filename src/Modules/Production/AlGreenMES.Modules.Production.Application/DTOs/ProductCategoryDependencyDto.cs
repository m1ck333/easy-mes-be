namespace AlGreenMES.Modules.Production.Application.DTOs;

public record ProductCategoryDependencyDto(
    Guid Id,
    Guid ProcessId,
    string? ProcessCode,
    Guid DependsOnProcessId,
    string? DependsOnProcessCode);
