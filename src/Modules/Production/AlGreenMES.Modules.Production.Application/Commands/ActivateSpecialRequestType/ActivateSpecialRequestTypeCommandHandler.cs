using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ActivateSpecialRequestType;

public class ActivateSpecialRequestTypeCommandHandler : IRequestHandler<ActivateSpecialRequestTypeCommand, Unit>
{
    private readonly ISpecialRequestTypeRepository _repository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public ActivateSpecialRequestTypeCommandHandler(ISpecialRequestTypeRepository repository, IProductionUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateSpecialRequestTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("SpecialRequestType", request.Id);

        type.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
