using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Production.Application.DTOs;

public record ProductCategoryProcessDto(
    Guid Id,
    Guid ProcessId,
    string? ProcessCode,
    string? ProcessName,
    ComplexityType? DefaultComplexity,
    int SequenceOrder);
