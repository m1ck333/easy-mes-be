using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ActivateProductCategory;

public class ActivateProductCategoryCommandHandler : IRequestHandler<ActivateProductCategoryCommand, Unit>
{
    private readonly IProductCategoryRepository _categoryRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public ActivateProductCategoryCommandHandler(IProductCategoryRepository categoryRepository, IProductionUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateProductCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("ProductCategory", request.Id);

        category.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
