using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Production.Application.Commands.AddSubProcess;

public class AddSubProcessCommandHandler : IRequestHandler<AddSubProcessCommand, SubProcessDto>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;
    private readonly DbContext _dbContext;

    public AddSubProcessCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
        _dbContext = (DbContext)unitOfWork;
    }

    public async Task<SubProcessDto> Handle(AddSubProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdWithSubProcessesAsync(request.ProcessId, cancellationToken)
            ?? throw new NotFoundException("Process", request.ProcessId);

        var subProcess = process.AddSubProcess(request.Name, request.SequenceOrder);

        // Explicitly mark as Added — navigation collection change detection
        // can miss this in EF Core 9 with backing field access mode
        _dbContext.Entry(subProcess).State = EntityState.Added;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subProcess.Adapt<SubProcessDto>();
    }
}
