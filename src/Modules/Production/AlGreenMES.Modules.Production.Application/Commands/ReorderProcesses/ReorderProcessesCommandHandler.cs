using AlGreenMES.Modules.Production.Application.Interfaces;
using AlGreenMES.Modules.Production.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Production.Application.Commands.ReorderProcesses;

public class ReorderProcessesCommandHandler : IRequestHandler<ReorderProcessesCommand, Unit>
{
    private readonly IProcessRepository _processRepository;
    private readonly IProductionUnitOfWork _unitOfWork;

    public ReorderProcessesCommandHandler(IProcessRepository processRepository, IProductionUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ReorderProcessesCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Items.Select(i => i.Id).ToList();
        var processes = await _processRepository.GetByIdsAsync(ids, cancellationToken);

        foreach (var item in request.Items)
        {
            var process = processes.FirstOrDefault(p => p.Id == item.Id);
            if (process != null)
            {
                process.Update(process.Code, process.Name, item.SequenceOrder);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
