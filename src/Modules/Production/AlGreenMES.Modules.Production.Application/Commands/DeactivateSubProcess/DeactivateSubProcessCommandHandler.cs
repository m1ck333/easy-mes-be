using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateSubProcess;

public class DeactivateSubProcessCommandHandler : IRequestHandler<DeactivateSubProcessCommand, Unit>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public DeactivateSubProcessCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateSubProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithSubProcessesAsync(request.ProcessId, cancellationToken)
            ?? throw new DomainException("PROCESS_NOT_FOUND", $"Process with id '{request.ProcessId}' was not found.");

        var subProcess = process.SubProcesses.FirstOrDefault(sp => sp.Id == request.SubProcessId)
            ?? throw new DomainException("SUBPROCESS_NOT_FOUND", $"Sub-process with id '{request.SubProcessId}' was not found.");

        subProcess.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
