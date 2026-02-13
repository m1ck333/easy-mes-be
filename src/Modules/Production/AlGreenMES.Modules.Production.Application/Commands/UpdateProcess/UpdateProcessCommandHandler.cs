using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.UpdateProcess;

public class UpdateProcessCommandHandler : IRequestHandler<UpdateProcessCommand, ProcessDto>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public UpdateProcessCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessDto> Handle(UpdateProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithSubProcessesAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Process", request.Id);

        process.Update(request.Name, request.SequenceOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return process.Adapt<ProcessDto>();
    }
}
