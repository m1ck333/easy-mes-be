using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateSpecialRequestType;

public class DeactivateSpecialRequestTypeCommandHandler : IRequestHandler<DeactivateSpecialRequestTypeCommand, Unit>
{
    private readonly ISpecialRequestTypeRepository _repository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public DeactivateSpecialRequestTypeCommandHandler(ISpecialRequestTypeRepository repository, IProductionUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateSpecialRequestTypeCommand request, CancellationToken cancellationToken)
    {
        var srt = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("SRT_NOT_FOUND", $"Special request type with id '{request.Id}' was not found.");

        srt.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
