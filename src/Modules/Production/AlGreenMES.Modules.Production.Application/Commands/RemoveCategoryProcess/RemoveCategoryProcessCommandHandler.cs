using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.RemoveCategoryProcess;

public class RemoveCategoryProcessCommandHandler : IRequestHandler<RemoveCategoryProcessCommand, ProductCategoryDetailDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public RemoveCategoryProcessCommandHandler(IProductCategoryRepository categoryRepository, IProductionUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductCategoryDetailDto> Handle(RemoveCategoryProcessCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdWithDetailsAsync(request.CategoryId, cancellationToken)
            ?? throw new DomainException("CATEGORY_NOT_FOUND", $"Product category with id '{request.CategoryId}' was not found.");

        category.RemoveProcess(request.ProcessId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductCategoryDetailDto(
            category.Id, category.TenantId, category.Name, category.Description, category.IsActive,
            category.Processes.Select(p => new ProductCategoryProcessDto(p.Id, p.ProcessId, p.Process?.Code, p.Process?.Name, p.DefaultComplexity, p.SequenceOrder)).ToList(),
            category.Dependencies.Select(d => new ProductCategoryDependencyDto(d.Id, d.ProcessId, d.Process?.Code, d.DependsOnProcessId, d.DependsOnProcess?.Code)).ToList());
    }
}
