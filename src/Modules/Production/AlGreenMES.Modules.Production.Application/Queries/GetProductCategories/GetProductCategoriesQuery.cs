using AlGreenMES.Modules.Production.Application.DTOs;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategories;

public record GetProductCategoriesQuery(Guid TenantId) : IRequest<IReadOnlyList<ProductCategoryDto>>;
