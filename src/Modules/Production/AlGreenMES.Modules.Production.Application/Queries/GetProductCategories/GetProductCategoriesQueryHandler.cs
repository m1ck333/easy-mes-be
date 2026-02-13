using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategories;

public class GetProductCategoriesQueryHandler : IRequestHandler<GetProductCategoriesQuery, IReadOnlyList<ProductCategoryDto>>
{
    private readonly IProductCategoryRepository _categoryRepository;

    public GetProductCategoriesQueryHandler(IProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<ProductCategoryDto>> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetByTenantIdAsync(request.TenantId, cancellationToken);

        return categories.Select(c => new ProductCategoryDto(
            c.Id, c.TenantId, c.Name, c.Description, c.IsActive
        )).ToList();
    }
}
