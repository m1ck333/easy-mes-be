using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Application.DTOs;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategories;

public record GetProductCategoriesQuery : PagedQuery<PagedResult<ProductCategoryDto>>
{
    public Guid TenantId { get; init; }
    public bool? IsActive { get; init; }
}
