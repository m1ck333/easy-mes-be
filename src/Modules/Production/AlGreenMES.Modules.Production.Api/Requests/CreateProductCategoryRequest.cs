namespace AlGreenMES.Modules.Production.Api.Requests;

public record CreateProductCategoryRequest(
    Guid TenantId,
    string Name,
    string? Description);
