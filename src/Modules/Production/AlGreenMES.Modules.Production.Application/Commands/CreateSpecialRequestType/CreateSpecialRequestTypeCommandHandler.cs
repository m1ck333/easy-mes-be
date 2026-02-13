using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.DTOs;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Entities;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.CreateSpecialRequestType;

public class CreateSpecialRequestTypeCommandHandler : IRequestHandler<CreateSpecialRequestTypeCommand, SpecialRequestTypeDto>
{
    private readonly ISpecialRequestTypeRepository _repository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public CreateSpecialRequestTypeCommandHandler(ISpecialRequestTypeRepository repository, IProductionUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SpecialRequestTypeDto> Handle(CreateSpecialRequestTypeCommand request, CancellationToken cancellationToken)
    {
        if (await _repository.ExistsByCodeAsync(request.Code, request.TenantId, cancellationToken))
            throw new DomainException("DUPLICATE_CODE", $"Special request type with code '{request.Code}' already exists.");

        var srt = SpecialRequestType.Create(request.TenantId, request.Code, request.Name, request.Description);

        if (request.AddsProcesses?.Count > 0)
            srt.SetAddsProcesses(request.AddsProcesses.ToArray());
        if (request.RemovesProcesses?.Count > 0)
            srt.SetRemovesProcesses(request.RemovesProcesses.ToArray());
        if (request.OnlyProcesses?.Count > 0)
            srt.SetOnlyProcesses(request.OnlyProcesses.ToArray());

        await _repository.AddAsync(srt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SpecialRequestTypeDto(
            srt.Id, srt.TenantId, srt.Code, srt.Name, srt.Description,
            srt.AddsProcesses, srt.RemovesProcesses, srt.OnlyProcesses,
            srt.IgnoresDependencies, srt.IsActive);
    }
}
