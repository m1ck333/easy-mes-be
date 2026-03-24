using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeleteProductCategory;

public class DeleteProductCategoryCommandHandler : IRequestHandler<DeleteProductCategoryCommand, DeleteProductCategoryResult>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;
    private readonly IReferenceCheckService _referenceCheck;

    public DeleteProductCategoryCommandHandler(
        IProductCategoryRepository categoryRepository,
        IProductionUnitOfWork unitOfWork,
        IReferenceCheckService referenceCheck)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _referenceCheck = referenceCheck;
    }

    public async Task<DeleteProductCategoryResult> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.Id);

        var refCount = await _referenceCheck.CountCategoryOrderReferencesAsync(request.Id, cancellationToken);

        if (refCount > 0 && !request.ForceDeactivate && !request.ForceDelete)
            return new DeleteProductCategoryResult(false, false, refCount);

        if (refCount > 0 && request.ForceDeactivate)
        {
            category.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new DeleteProductCategoryResult(false, true, refCount);
        }

        // ForceDelete or no references: nullify references then hard delete
        if (refCount > 0)
            await _referenceCheck.NullifyCategoryReferencesAsync(request.Id, cancellationToken);

        _categoryRepository.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new DeleteProductCategoryResult(true, false, 0);
    }
}
