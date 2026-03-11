namespace AlGreenMES.Modules.Production.Api.Requests;

public record UpdateProductCategoryRequest(
    string Name,
    string? Description,
    int? DefaultWarningDays,
    int? DefaultCriticalDays,
    List<CategoryProcessInput>? Processes,
    List<CategoryDependencyInput>? Dependencies);
