namespace AlGreenMES.Modules.Production.Api.Requests;

public record UpdateProductCategoryRequest(
    string Name,
    string? Description);
