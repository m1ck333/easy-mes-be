using AlGreenMES.BuildingBlocks.Common.Pagination;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategories;

public class GetProductCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQuery, PagedResult<ProductCategoryDto>>
{
    private readonly IProductCategoryRepository _categoryRepository;

    public GetProductCategoriesQueryHandler(IProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _categoryRepository.GetPagedAsync(
            request.TenantId, request.IsActive, request.Search,
            request.GetPage(), request.GetPageSize(), cancellationToken);

        return result.MapItems(c => c.Adapt<ProductCategoryDto>());
    }
}
