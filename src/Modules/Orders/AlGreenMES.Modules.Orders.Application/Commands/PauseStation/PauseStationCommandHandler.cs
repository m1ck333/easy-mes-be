using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Enums;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.PauseStation;

public class PauseStationCommandHandler : IRequestHandler<PauseStationCommand>
{
    private readonly IOrderItemProcessRepository _processRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public PauseStationCommandHandler(
        IOrderItemProcessRepository processRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _processRepository = processRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(PauseStationCommand request, CancellationToken cancellationToken)
    {
        var activeProcesses = await _processRepository.GetInProgressByProcessIdAsync(
            request.ProcessId, request.TenantId, cancellationToken);

        foreach (var process in activeProcesses)
        {
            var hasSubProcesses = process.SubProcesses.Any(sp => !sp.IsWithdrawn);
            if (hasSubProcesses)
            {
                var activeSub = process.SubProcesses
                    .FirstOrDefault(sp => sp.Status == SubProcessStatus.InProgress);
                if (activeSub != null)
                {
                    var openLog = activeSub.GetOpenLog();
                    if (openLog != null)
                    {
                        openLog.End();
                        if (openLog.DurationMinutes.HasValue)
                            activeSub.AddDuration(openLog.DurationMinutes.Value);
                    }
                }
            }
            else
            {
                if (!process.PausedAt.HasValue)
                    process.Pause();
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
