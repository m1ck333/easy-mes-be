using AlGreenMES.BuildingBlocks.Common.Exceptions;
using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.DeactivateProcess;

public class DeactivateProcessCommandHandler : IRequestHandler<DeactivateProcessCommand, Unit>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public DeactivateProcessCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateProcessCommand request, CancellationToken cancellationToken)
    {
        var process = await _processRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Process", request.Id);

        process.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
