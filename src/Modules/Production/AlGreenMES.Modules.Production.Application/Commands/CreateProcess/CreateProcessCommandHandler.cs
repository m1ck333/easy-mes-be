using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using Mapster;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateProcess;

public class CreateProcessCommandHandler : IRequestHandler<CreateProcessCommand, ProcessDto>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public CreateProcessCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProcessDto> Handle(CreateProcessCommand request, CancellationToken cancellationToken)
    {
        var codeExists = await _processRepository.ExistsByCodeAsync(request.Code, request.TenantId, cancellationToken);
        if (codeExists)
            throw new DomainException("PROCESS_CODE_EXISTS", $"A process with code '{request.Code}' already exists.");

        var process = Process.Create(request.TenantId, request.Code, request.Name, request.SequenceOrder, request.CreatedByUserId);

        await _processRepository.AddAsync(process, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return process.Adapt<ProcessDto>();
    }
}
