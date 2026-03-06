namespace AlGreenMES.Modules.Production.Application.DTOs;

public record ProductCategoryDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record ProductCategoryDetailDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<ProductCategoryProcessDto> Processes,
    List<ProductCategoryDependencyDto> Dependencies);
