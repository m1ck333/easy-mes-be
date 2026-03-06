using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.AddCategoryDependency;

public class AddCategoryDependencyCommandHandler : IRequestHandler<AddCategoryDependencyCommand, ProductCategoryDetailDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public AddCategoryDependencyCommandHandler(IProductCategoryRepository categoryRepository, IProductionUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductCategoryDetailDto> Handle(AddCategoryDependencyCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdWithDetailsAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.CategoryId);

        category.AddDependency(request.ProcessId, request.DependsOnProcessId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Clear change tracker to force fresh load with all navigation properties
        _unitOfWork.ClearChangeTracker();

        category = await _categoryRepository.GetByIdWithDetailsAsync(request.CategoryId, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.CategoryId);

        return category.Adapt<ProductCategoryDetailDto>();
    }
}
