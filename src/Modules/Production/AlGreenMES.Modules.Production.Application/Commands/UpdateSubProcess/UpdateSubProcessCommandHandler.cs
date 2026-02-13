using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateSubProcess;

public class UpdateSubProcessCommandHandler : IRequestHandler<UpdateSubProcessCommand, SubProcessDto>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public UpdateSubProcessCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubProcessDto> Handle(UpdateSubProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithSubProcessesAsync(request.ProcessId, cancellationToken)
            ?? throw new DomainException("PROCESS_NOT_FOUND", $"Process with id '{request.ProcessId}' was not found.");

        var subProcess = process.SubProcesses.FirstOrDefault(sp => sp.Id == request.SubProcessId)
            ?? throw new DomainException("SUBPROCESS_NOT_FOUND", $"Sub-process with id '{request.SubProcessId}' was not found.");

        subProcess.Update(request.Name, request.SequenceOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubProcessDto(subProcess.Id, subProcess.ProcessId, subProcess.Name, subProcess.SequenceOrder, subProcess.IsActive);
    }
}
