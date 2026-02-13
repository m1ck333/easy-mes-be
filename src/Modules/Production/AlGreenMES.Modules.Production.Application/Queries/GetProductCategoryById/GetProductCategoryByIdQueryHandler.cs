using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Queries.GetProductCategoryById;

public class GetProductCategoryByIdQueryHandler : IRequestHandler<GetProductCategoryByIdQuery, ProductCategoryDetailDto?>
{
    private readonly IProductCategoryRepository _categoryRepository;

    public GetProductCategoryByIdQueryHandler(IProductCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductCategoryDetailDto?> Handle(GetProductCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (category is null) return null;

        return new ProductCategoryDetailDto(
            category.Id, category.TenantId, category.Name, category.Description, category.IsActive,
            category.Processes.Select(p => new ProductCategoryProcessDto(p.Id, p.ProcessId, p.Process?.Code, p.Process?.Name, p.DefaultComplexity, p.SequenceOrder)).ToList(),
            category.Dependencies.Select(d => new ProductCategoryDependencyDto(d.Id, d.ProcessId, d.Process?.Code, d.DependsOnProcessId, d.DependsOnProcess?.Code)).ToList());
    }
}
