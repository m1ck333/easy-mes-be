using AlGreenMES.Modules.Production.Domain.Enums;

namespace AlGreenMES.Modules.Production.Api.Requests;

public record AddCategoryProcessRequest(
    Guid ProcessId,
    int SequenceOrder,
    ComplexityType? DefaultComplexity);
