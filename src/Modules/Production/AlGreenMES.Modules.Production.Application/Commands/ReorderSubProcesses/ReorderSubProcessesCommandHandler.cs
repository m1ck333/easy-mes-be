using AlGreenMES.Modules.Production.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlGreenMES.Modules.Production.Application.Commands.ReorderSubProcesses;

public class ReorderSubProcessesCommandHandler : IRequestHandler<ReorderSubProcessesCommand, Unit>
{
    private readonly IProductionUnitOfWork _unitOfWork;
    private readonly DbContext _dbContext;

    public ReorderSubProcessesCommandHandler(IProductionUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _dbContext = (DbContext)unitOfWork;
    }

    public async Task<Unit> Handle(ReorderSubProcessesCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Items.Select(i => i.Id).ToList();
        var subProcesses = await _dbContext.Set<Domain.Entities.SubProcess>()
            .Where(s => ids.Contains(s.Id) && s.ProcessId == request.ProcessId)
            .ToListAsync(cancellationToken);

        // First pass: shift to high temp values to avoid unique (ProcessId, SequenceOrder) constraint
        // violations when EF Core sends individual UPDATE statements within the transaction.
        for (int i = 0; i < subProcesses.Count; i++)
        {
            subProcesses[i].Update(subProcesses[i].Name, 10000 + i);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Second pass: set final sequence orders
        foreach (var item in request.Items)
        {
            var sub = subProcesses.FirstOrDefault(s => s.Id == item.Id);
            if (sub != null)
            {
                sub.Update(sub.Name, item.SequenceOrder);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
