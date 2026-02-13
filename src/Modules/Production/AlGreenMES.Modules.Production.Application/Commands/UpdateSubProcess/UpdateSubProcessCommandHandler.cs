using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
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
            ?? throw new NotFoundException("Process", request.ProcessId);

        var subProcess = process.SubProcesses.FirstOrDefault(sp => sp.Id == request.SubProcessId)
            ?? throw new NotFoundException("SubProcess", request.SubProcessId);

        subProcess.Update(request.Name, request.SequenceOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subProcess.Adapt<SubProcessDto>();
    }
}
