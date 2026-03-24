using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeleteProcess;

public class DeleteProcessCommandHandler : IRequestHandler<DeleteProcessCommand, DeleteProcessResult>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;
    private readonly IReferenceCheckService _referenceCheck;

    public DeleteProcessCommandHandler(
        IProcessRepository processRepository,
        IProductionUnitOfWork unitOfWork,
        IReferenceCheckService referenceCheck)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
        _referenceCheck = referenceCheck;
    }

    public async Task<DeleteProcessResult> Handle(DeleteProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Process", request.Id);

        var refCount = await _referenceCheck.CountProcessOrderReferencesAsync(request.Id, cancellationToken);

        if (refCount > 0 && !request.ForceDeactivate && !request.ForceDelete)
            return new DeleteProcessResult(false, false, refCount);

        if (refCount > 0 && request.ForceDeactivate)
        {
            process.Deactivate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new DeleteProcessResult(false, true, refCount);
        }

        if (refCount > 0)
            await _referenceCheck.NullifyProcessReferencesAsync(request.Id, cancellationToken);

        _processRepository.Remove(process);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new DeleteProcessResult(true, false, 0);
    }
}
