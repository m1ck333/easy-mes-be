using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.RemoveCategoryDependency;

public class RemoveCategoryDependencyCommandHandler : IRequestHandler<RemoveCategoryDependencyCommand, ProductCategoryDetailDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public RemoveCategoryDependencyCommandHandler(IProductCategoryRepository categoryRepository, IProductionUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductCategoryDetailDto> Handle(RemoveCategoryDependencyCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdWithDetailsAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.CategoryId);

        category.RemoveDependency(request.DependencyId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Adapt<ProductCategoryDetailDto>();
    }
}
