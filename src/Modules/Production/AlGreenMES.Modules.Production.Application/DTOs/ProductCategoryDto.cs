namespace AlGreenMES.Modules.Production.Application.DTOs;

public record ProductCategoryDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    bool IsActive);

public record ProductCategoryDetailDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    bool IsActive,
    List<ProductCategoryProcessDto> Processes,
    List<ProductCategoryDependencyDto> Dependencies);
