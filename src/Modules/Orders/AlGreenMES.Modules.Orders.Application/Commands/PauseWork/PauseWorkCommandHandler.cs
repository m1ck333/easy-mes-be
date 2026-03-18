using AlGreenMES.Modules.Orders.Application.Interfaces;
using AlGreenMES.Modules.Orders.Domain.Repositories;
using MediatR;

namespace AlGreenMES.Modules.Orders.Application.Commands.PauseWork;

public class PauseWorkCommandHandler : IRequestHandler<PauseWorkCommand, Unit>
{
    private readonly IOrderItemSubProcessRepository _subProcessRepository;
    private readonly IOrdersUnitOfWork _unitOfWork;

    public PauseWorkCommandHandler(
        IOrderItemSubProcessRepository subProcessRepository,
        IOrdersUnitOfWork unitOfWork)
    {
        _subProcessRepository = subProcessRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(PauseWorkCommand request, CancellationToken cancellationToken)
    {
        var activeLogs = await _subProcessRepository.GetActiveLogsByUserIdAsync(request.UserId, cancellationToken);

        foreach (var log in activeLogs)
        {
            log.End();
            if (log.DurationMinutes.HasValue)
                log.OrderItemSubProcess.AddDuration(log.DurationMinutes.Value);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
