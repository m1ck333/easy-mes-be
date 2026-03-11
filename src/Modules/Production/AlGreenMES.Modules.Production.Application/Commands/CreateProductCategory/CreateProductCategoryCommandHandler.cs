using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateProductCategory;

public class CreateProductCategoryCommandHandler : IRequestHandler<CreateProductCategoryCommand, ProductCategoryDetailDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public CreateProductCategoryCommandHandler(IProductCategoryRepository categoryRepository, IProductionUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductCategoryDetailDto> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var nameExists = await _categoryRepository.ExistsByNameAsync(request.Name, request.TenantId, cancellationToken);
        if (nameExists)
            throw new DomainException("CATEGORY_NAME_EXISTS", $"A product category with name '{request.Name}' already exists.");

        var category = ProductCategory.Create(request.TenantId, request.Name, request.Description, request.CreatedByUserId, request.DefaultWarningDays, request.DefaultCriticalDays);

        if (request.Processes is { Count: > 0 })
        {
            foreach (var proc in request.Processes)
                category.AddProcess(proc.ProcessId, proc.SequenceOrder, proc.DefaultComplexity);
        }

        if (request.Dependencies is { Count: > 0 })
        {
            foreach (var dep in request.Dependencies)
                category.AddDependency(dep.ProcessId, dep.DependsOnProcessId);
        }

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties for response mapping
        var reloaded = await _categoryRepository.GetByIdWithDetailsAsync(category.Id, cancellationToken);
        return reloaded!.Adapt<ProductCategoryDetailDto>();
    }
}
