using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateProductCategory;

public class UpdateProductCategoryCommandHandler : IRequestHandler<UpdateProductCategoryCommand, ProductCategoryDetailDto>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public UpdateProductCategoryCommandHandler(IProductCategoryRepository categoryRepository, IProductionUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductCategoryDetailDto> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.Id);

        category.Update(request.Name, request.Description);

        if (request.Processes is not null)
        {
            category.ReplaceProcesses(request.Processes.Select(p =>
                (p.ProcessId, p.SequenceOrder, p.DefaultComplexity)));
        }

        if (request.Dependencies is not null)
        {
            category.ReplaceDependencies(request.Dependencies.Select(d =>
                (d.ProcessId, d.DependsOnProcessId)));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with navigation properties for response mapping
        var reloaded = await _categoryRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        return reloaded!.Adapt<ProductCategoryDetailDto>();
    }
}
