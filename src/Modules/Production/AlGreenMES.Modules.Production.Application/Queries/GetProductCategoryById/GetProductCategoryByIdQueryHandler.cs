using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategoryById;

public class GetProductCategoryByIdQueryHandler : IRequestHandler<GetProductCategoryByIdQuery, ProductCategoryDetailDto>
{
    private readonly IProductCategoryRepository _categoryRepository;

    public GetProductCategoryByIdQueryHandler(IProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductCategoryDetailDto> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.Id);

        return category.Adapt<ProductCategoryDetailDto>();
    }
}
